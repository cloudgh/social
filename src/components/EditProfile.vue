<template>
    <div v-if="visible" class="modal-overlay" :class="{ 'dark-mode': isDarkMode }">
      <div class="modal">
        <div class="modal-header">
          <h3>{{ title }}</h3>
          <!-- <button @click="close" class="close-button">&times;</button> -->
        </div>
        <div class="modal-body">
          <slot name="body"></slot>
        </div>
        <div class="modal-footer">
          <slot name="btn"></slot>
        </div>
      </div>
    </div>
  </template>
  
  <script>
  export default {
    props: {
      title: {
        type: String,
        default: 'Modal Title'
      },
      visible: {
        type: Boolean,
        default: false
      }
    },
    computed: {
      isDarkMode() {
        return this.$store.state.isDarkMode;
      },
    }
  };
  </script>
  
<style scoped lang="scss">
  .modal-overlay {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 8;
  }
  .modal {
    background: white;
    padding: 20px;
    border-radius: 8px;
    width: max-content;
    max-width: 100%;
  }
  .dark-mode .modal{
    background-color: rgba(23, 23, 23);
    color: aliceblue;
  }
  
  .modal-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding-bottom: 10px;
  }
  .modal-footer{
    display: flex;
    padding-top: 10px;
    justify-content: space-between;
    align-items: center;
  }
</style>
  