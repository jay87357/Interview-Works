using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

public static class EfCoreExtensions
{
    /// <summary>
    /// Upsert 多筆資料，回傳所有新增及更新的主鍵清單
    /// </summary>
    /// <typeparam name="T">實體類別</typeparam>
    /// <typeparam name="TK">主鍵類型</typeparam>
    /// <param name="context">DbContext 實例</param>
    /// <param name="entities">要新增或更新的資料</param>
    /// <returns>受影響的主鍵清單（包含新增及更新）</returns>
    public static async Task<List<TK>> UpsertRangeAsync<T, TK>(
        this DbContext context,
        IEnumerable<T> entities)
        where T : class
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        var entityList = entities.ToList();
        if (!entityList.Any())
            return new List<TK>();

        var entityType = context.Model.FindEntityType(typeof(T));
        if (entityType == null)
            throw new InvalidOperationException($"找不到實體 {typeof(T).Name} 的模型資訊");

        var key = entityType.FindPrimaryKey();
        if (key == null)
            throw new InvalidOperationException($"實體 {typeof(T).Name} 沒有定義主鍵");

        var keyProp = key.Properties.FirstOrDefault()?.PropertyInfo;
        if (keyProp == null)
            throw new InvalidOperationException($"無法取得 {typeof(T).Name} 的主鍵屬性資訊");


        var ids = entityList
            .Select(e => keyProp.GetValue(e))
            .Where(id => id != null)
            .Select(id => (TK)Convert.ChangeType(id, typeof(TK)))
            .ToList();

        // 取得主鍵屬性名稱
        var keyName = key.Properties.First().Name;

        // Expression 參數
        var parameter = Expression.Parameter(typeof(T), "e");
        var property = Expression.Property(parameter, keyName);

        var containsMethod = typeof(List<TK>).GetMethod("Contains", new[] { typeof(TK) });

        var idsConstant = Expression.Constant(ids);

        var containsCall = Expression.Call(idsConstant, containsMethod, property);

        var lambda = Expression.Lambda<Func<T, bool>>(containsCall, parameter);

        var existingIds = await context.Set<T>()
            .Where(lambda)
            .Select(Expression.Lambda<Func<T, TK>>(property, parameter))
            .ToListAsync();

        var affectedKeys = new List<TK>();

        foreach (var entity in entityList)
        {
            var idObj = keyProp.GetValue(entity);
            var id = idObj == null ? default(TK) : (TK)Convert.ChangeType(idObj, typeof(TK));

            if (idObj == null || !existingIds.Contains(id))
            {
                context.Add(entity);
                affectedKeys.Add(id);
            }
            else
            {
                context.Update(entity);
                affectedKeys.Add(id);
            }
        }

        await context.SaveChangesAsync();

        // SaveChanges 後，新增資料的主鍵會寫回實體，再更新 affectedKeys 裡的新增主鍵
        for (int i = 0; i < entityList.Count; i++)
        {
            var idAfterSave = (TK)Convert.ChangeType(keyProp.GetValue(entityList[i]), typeof(TK));
            affectedKeys[i] = idAfterSave;
        }

        return affectedKeys;
    }
}
