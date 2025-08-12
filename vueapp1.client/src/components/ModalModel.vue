<template>
  <div>
    <div :id="id"
         class="modal"
         :style="{ zIndex: zindex }"
         data-bs-backdrop="static"
         data-bs-keyboard="false"
         tabindex="-1"
         ref="modalEl">
      <div class="modal-dialog modal-dialog-centered modal-xl modal-dialog-scrollable"
           :style="{ width: width || undefined, position: isdrag ? 'absolute' : undefined }"
           ref="dialogEl">
        <div class="modal-content"
             ref="contentEl"
             :style="{ resize: isresize ? 'both' : 'none', overflow: isresize ? 'auto' : undefined }">
          <div class="modal-header" ref="headerEl">
            <slot name="header">
              <h5 class="modal-title">{{ title }}</h5>
              <button type="button" class="btn-close" aria-label="Close" @click="hide"></button>
            </slot>
          </div>
          <div class="modal-body">
            <slot name="body"></slot>
          </div>
          <div class="modal-footer">
            <slot name="footer"></slot>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
  import { ref, onMounted, onBeforeUnmount } from 'vue'

  const props = defineProps({
    title: { type: String, default: '' },
    isdrag: { type: Boolean, default: false },
    isresize: { type: Boolean, default: false },
    zindex: { type: Number, default: 10000 },
    width: { type: String, default: '' }
  })

  const emit = defineEmits(['close'])

  const id = ref('MyModal_' + Math.floor(Math.random() * 10000))
  const isShow = ref(false)

  const modalEl = ref(null)
  const dialogEl = ref(null)
  const headerEl = ref(null)

  let modalInstance = null

  function hide() {
    modalInstance?.hide()
  }

  function show() {
    modalInstance?.show()
  }

  onMounted(() => {
    // 使用全域 bootstrap.Modal
    modalInstance = new bootstrap.Modal(modalEl.value, {
      backdrop: props.isdrag ? false : 'static',
      keyboard: false
    })

    modalEl.value.addEventListener('hidden.bs.modal', () => {
      emit('close')
      isShow.value = false
    })

    modalEl.value.addEventListener('shown.bs.modal', () => {
      isShow.value = true
    })

    // 拖曳功能
    if (props.isdrag) {
      let offsetX = 0
      let offsetY = 0
      let dragging = false

      const onMouseDown = (e) => {
        dragging = true
        const rect = dialogEl.value.getBoundingClientRect()
        offsetX = e.clientX - rect.left
        offsetY = e.clientY - rect.top
        document.addEventListener('mousemove', onMouseMove)
        document.addEventListener('mouseup', onMouseUp)
      }

      const onMouseMove = (e) => {
        if (!dragging) return
        dialogEl.value.style.left = e.clientX - offsetX + 'px'
        dialogEl.value.style.top = e.clientY - offsetY + 'px'
      }

      const onMouseUp = () => {
        dragging = false
        document.removeEventListener('mousemove', onMouseMove)
        document.removeEventListener('mouseup', onMouseUp)
      }

      headerEl.value.style.cursor = 'move'
      headerEl.value.addEventListener('mousedown', onMouseDown)

      // 清理事件
      onBeforeUnmount(() => {
        headerEl.value.removeEventListener('mousedown', onMouseDown)
      })
    }

    // 設定 z-index
    if (props.zindex) {
      modalEl.value.style.zIndex = props.zindex
    }
  })

  onBeforeUnmount(() => {
    modalInstance?.dispose()
  })

  defineExpose({ show, hide })
</script>
