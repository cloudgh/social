<template>
  <div class="modal" id="modal" v-if="isVisible">
    <div class="modal-content">
      <span class="close" @click="closeModal">&times;</span>
      <div class="commet-text" v-if="comments.length==0">Нет комментариев</div>
      <div v-for="comment in comments" class="commet">
        <div class="commet-text" v-if="comments.length!=0">
          <div class="commet-autor">
            <span class="commet-autor__icon">&#129430;</span>
            
            <span v-if="comment.author">{{comment.author.username }}</span>
            <span class="formdata">{{formattedDate(comment.created)}}</span>
            
          </div>
          
          <h5>{{ comment.text }}</h5>
        </div>
       
      </div>
    </div>
  </div>
</template>


<script>
import axios from 'axios';
export default {
  props: ['postId'],
  data() {
    return {
      isVisible: false,
      comments: []
    };
  },
  
  methods: {
    
    formattedDate(dateString) {
      if (!dateString) return '';
      const options = { 
        day: 'numeric', 
        month: 'long', 
        year: 'numeric'
      };
      return new Date(dateString).toLocaleDateString('ru-RU', options);
    },
    async loadComments() {
      try {
        const response = await axios.get(`https://localhost:7297/api/posts/${this.postId}`, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });
        this.comments = response.data.comments;
        this.isVisible = true;
      } catch (error) {
        console.error('Ошибка загрузки комментариев:', error);
      }
    },
    closeModal() {
      this.isVisible = false;
      this.comments = [];
      this.$emit('close');
    }
  },
 
  mounted() {
    this.loadComments();
  }
};

</script>


<style lang="scss" scoped>
.commet{
  display: flex;
  margin-bottom: 10px;
  border-radius: 20px;
  background-color: #dcdcdc3a;
  width: 100%;
  border: 2px solid #a4a2a23c;
  padding: 10px ;
  display: flex;
  position: relative;

.commet-text{
  text-align: justify;
  width: 100%;
  
  .commet-autor{
    display: grid;
    grid-template-columns: max-content max-content 1fr;
    grid-template-rows: 1fr 1fr;
    font-size: 12px;
    &__icon{
      grid-row: 1 / 3;
      font-size: 20px;
    }
    
  }
  h5{
    width: 90%;
    margin: 15px auto 5px auto;
  }
}
}
.modal {
  display: table;
  position: fixed;
  top: 50%;
  left: 50%;; 
  transform: translate(-50%, -50%);
  padding: 20px;
  background-color: white;
  width: 60vw;
  max-height: 80vh;
  overflow-y: auto;
  margin-top: 40px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.3);
  max-height: 80vh; 
  overflow-y: auto;
  z-index: 3;
  animation: bendText 2s 1;;

  
}
@keyframes bendText{
  0% {
    transform: translate(0%, 0%) scale(0.1);
  }
  100% {
    transform: translate(-50%, -50%) scale(1);
  }
}
.modal-content {
  margin: 0 auto;
  max-height: 88vh;
  overflow-y: auto;
}

.close {
  position: absolute;
  top: -0.4rem;
  right: 0.2rem;
  cursor: pointer;
  font-size: 20pt;
}

  .dark-mode .modal{
    background-color: #575757;
  }
</style>
