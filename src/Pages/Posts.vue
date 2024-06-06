<template>
  <div class="app1" :class="{ 'dark-mode': isDarkMode , 'modal_open': isModalOpen }">
    <div v-if="isLoading" class="loading-overlay">
      <div v-if="errorLoadingData" class="error-message">
        <p>Ошибка загрузки данных. Пожалуйста, проверьте соединение и попробуйте снова.</p>
      </div>
      <div class="spinner">
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
        <div></div>
      </div>
      <h2>Loading...</h2>
    </div>
    <Lamp></Lamp>
    <Menu></Menu>
    <Menu1></Menu1>
    <div class="book">
      <div class="post-cont post-cont1">
        <h1>Посты</h1>
        <div v-for="(post, index) in firstColumn" :key="post.id" class="post">
          <div class="post-text">
            <div class="post-autor">
              <span class="post-autor__icon">&#127183;</span>
              
              <span>{{ post.author.username }}</span>
              <span class="formdata">{{formattedDate(post.created)}}</span>
              <div  v-if="post.authorId == my_id || this.isadmin === 'true'" class="edit-icon">
                <span>…</span>
                <div class="edit-post">
                  <div @click="deletePost(post.id)">Удалить пост</div>
                  <div v-if="post.authorId== my_id " @click="openEditPostModal(post.id, post.text)">Редактировать пост</div>
                  <EditPost :title="'Edit Post'" v-model:visible="showEditPostModal" :postId="selectedPostId">
                    <template #body>
                      <textarea rows="5" cols="33" style ="padding:10px;" v-model="editPostText" placeholder="Enter your post"></textarea>
                    </template>  
                  <!-- </div> -->
                    <template #btn>
                      <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="editPost">Submit</button>
                      <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="showEditPostModal = false">Close</button>
                    </template>
                </EditPost>
                </div>
              </div>
            </div>
            
            <h4>{{ post.text }}</h4>
            <div class="user_btn">
              <button @click="userpage(post.author.username)">Перейти к пользователю</button>
              <button @click="openModal(post.id)">Показать комментарии</button>
            </div>
        </div>
        </div>
      </div>
      <div class="sq"></div>
      <div class="post-cont post-cont2">
        <h1 class="post2">Посты</h1>
        <div v-for="(post, index) in secondColumn" :key="post.id" class="post post2">
          <div class="post-text">
            <div class="post-autor">
              <span class="post-autor__icon">&#127183;</span>
              
              <span>{{ post.author.username }}</span>
              <span class="formdata">{{formattedDate(post.created)}}</span>
              <div  v-if="post.authorId== my_id ||  this.isadmin === 'true'" class="edit-icon">
                <span>…</span>
                <div class="edit-post">
                  <div @click="deletePost(post.id)">Удалить пост</div>
                  <div v-if="post.authorId== my_id" @click="openEditPostModal(post.id, post.text)">Редактировать пост</div>
                  <EditPost :title="'Edit Post'" v-model:visible="showEditPostModal" :postId="selectedPostId">
                    <template #body>
                      <textarea rows="5" cols="33" style ="padding:10px;" v-model="editPostText" placeholder="Enter your post"></textarea>
                    </template>  
                  <!-- </div> -->
                    <template #btn>
                      <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="editPost">Submit</button>
                      <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="showEditPostModal = false">Close</button>
                    </template>
                </EditPost>
                </div>
              </div>
            </div>
            
            <h4>{{ post.text }}</h4>
            <div class="user_btn">
              <button @click="userpage(post.author.username)">Перейти к пользователю</button>
              <button @click="openModal(post.id)">Показать комментарии</button>
            </div>
            
          </div>
        </div>
      </div>
    </div>

    <Modal v-if="isModalOpen" @close="closeModal" :post-id="currentPostId"></Modal>
  </div>
</template>


<script>
import axios from 'axios';
import{ ElNotification} from 'element-plus' 
import Modal from '@/components/Comments.vue';
import Menu from '@/components/Menu.vue';
import Menu1 from '@/components/Menu1.vue';
import Lamp from '@/components/Lamp.vue';
import EditPost from '@/components/EditPost.vue';
export default {
  components: {
    Modal,
    Menu,
    Lamp,
    EditPost,
    Menu1
  },
  data() {
    return {
      posts: [],
      isModalOpen: false,
      currentPostId: null,
      isLoading: true,
      errorLoadingData: false,
      my_id:null,
      isadmin:null,
      showEditPostModal:false
    };
  },
  async mounted() {

    this.isadmin = localStorage.getItem('isAdmin');
    this.my_id=localStorage.getItem('userId');
    
    try {
      const response = await axios.get('https://localhost:7297/api/feed', {
        headers: {
          Authorization: `Bearer ${localStorage.getItem('token')}`
        }
      });
      this.posts = response.data;
      this.isLoading = false;
    } catch (error) {
      this.errorLoadingData = true;
      this.isLoading = false;
    }
  },
  methods: {
    async deletePost(postId) {
      try {
        await axios.delete(`https://localhost:7297/api/posts/${postId}`);
        this.handlePostDeleted(postId);
      } catch (error) {
        this.errorMessage = error.response ? error.response.data.message : 'Error deleting post';
        ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
      }
    },
    handlePostDeleted(postId) {
      this.posts = this.posts.filter(post => post.id !== postId);
    },
    async editPost() {
      if (!this.selectedPostId) return;
      try {
       
        await axios.put(`https://localhost:7297/api/posts/${this.selectedPostId}`, {
          text: this.editPostText
        });
        this.showEditPostModal = false;
        this.handlePostEdited(this.selectedPostId, this.editPostText);
        this.editPostText = ''; 
      } catch (error) {
        this.errorMessage = error.response ? error.response.data.message : 'Error editing post';
        ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
      }
    },
    handlePostEdited(postId, updatedText) {
      const postIndex = this.posts.findIndex(post => post.id === postId);
      if (postIndex !== -1) {
        this.posts[postIndex].text = updatedText;
      }
    },
    openEditPostModal(postId, postText) {
      this.selectedPostId = postId;
      this.editPostText = postText;
      this.showEditPostModal = true;
    },
    formattedDate(dateString) {
      if (!dateString) return '';
      const options = { 
        day: 'numeric', 
        month: 'long', 
        year: 'numeric'
      };
      return new Date(dateString).toLocaleDateString('ru-RU', options);
    },
    openModal(postId) {
      this.currentPostId = postId;
      this.isModalOpen = true;
    },
    closeModal() {
      this.isModalOpen = false;
      this.currentPostId = null;
    },
    userpage(username) {
      this.$router.push({ name: 'user', params: { username: username } });
    }
  },
  computed: {
    isDarkMode() {
      return this.$store.state.isDarkMode;
    },
    firstColumn() {
      return this.posts.slice(0, Math.ceil(this.posts.length / 2));
    },
    secondColumn() {
      return this.posts.slice(Math.ceil(this.posts.length / 2));
    },
  },
};

</script>

<style lang="scss" scoped>
.user_btn{
  margin-top: 10px;
  display: flex;
  justify-content: space-around;
  button{
    background-color: #dcdcdc3a;
    border: 2px solid #a4a2a23c;
    border-radius:20px ;
  }
}
.formdata{
  padding-left: 2px;
}
.dark-mode {
   .post .post-text .post-autor .edit-post{
    background-color: rgb(92, 91, 91);
  }
}
  
.post{
  display: flex;
  flex-direction: column;
  padding: 3px;
  margin-bottom: 10px;
  border-radius: 20px;
  background-color: #dcdcdc3a;
  width: 100%;
  border: 2px solid #a4a2a23c;
  padding: 10px ;
  .post-text{
    text-align: justify  ;
    .post-autor{
      display: grid;
      grid-template-columns: max-content max-content 1fr;
      grid-template-rows: 1fr 1fr;
      font-size: 12px;
      &__icon{
        grid-row: 1 / 3;
        font-size: 24px;
      }
      .edit-icon {
        display: flex;
        justify-content: flex-end;
        grid-column: 3;
        grid-row: 1 / 3;
        cursor: pointer;
        span{
          font-size: 20px;
          transform: rotate(90deg)
        }
      }
      .edit-post{
        flex-direction: column;
        display: none;
        gap:5px;
        padding: 5px;
        border-radius:10px;
        border: 2px solid #a4a2a23c;
        position: absolute;
        margin-top: 65px;
        justify-self: center;
        align-self: center;
        background-color: #fff;
        div{
          padding: 2px;
        }
        div:hover{
          background-color: #9d9b9b3a;
          border-radius: 5px;
        }
      }
      .edit-icon:hover{
        .edit-post{
          display: flex;
          cursor: pointer;
        }
      }
    }
    h4{
      width: 90%;
      margin: 15px auto 5px auto;
    }
  }
  .comments{
    display: grid;
    grid-template-columns: max-content 1fr max-content ;
    padding-left: 5px;
    .function_span{
      display: flex;
      justify-content: flex-end;
      grid-column: 3;
      color: rgb(140, 223, 7);
      cursor: pointer;
      font-size: 25px;
      font-weight: bolder;
      span{
        display: flex;
        justify-content: center;
        align-items: center;
        border: 2px solid #a4a2a23c;
        border-radius: 50%;
        height: 25px;
        width: 25px;
      }
      
    }
  }
}
.error-message {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  padding: 20px;
  background-color: #faccff;
  border: 1px solid #ae00ff;
  border-radius: 5px;
  text-align: center;
  z-index: 2;
  p{
    text-align: center;
  }
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 50%;
  transform: translateX(-50%);
  width: 70vw;
  height: 100vh;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: rgba(255, 255, 255);
  z-index: 3;
  @media (max-width: 500px) {
    width: 100vw;
  }
  h2{
    margin-top: 30px;
  }
}
.dark-mode .loading-overlay{
  background-color: rgba(23, 23, 23);
  left: 47%;
  transform: translateX(-47%);
}
.spinner {
 width: 88px;
 height: 88px;
 animation: spinner-y0fdc1 2s infinite ease;
 transform-style: preserve-3d;
 div:nth-of-type(1) {
  transform: translateZ(-44px) rotateY(180deg);
  }

  div:nth-of-type(2) {
  transform: rotateY(-270deg) translateX(50%);
  transform-origin: top right;
  }

  div:nth-of-type(3) {
  transform: rotateY(270deg) translateX(-50%);
  transform-origin: center left;
  }

  div:nth-of-type(4) {
  transform: rotateX(90deg) translateY(-50%);
  transform-origin: top center;
  }

  div:nth-of-type(5) {
  transform: rotateX(-90deg) translateY(50%);
  transform-origin: bottom center;
  }

  div:nth-of-type(6) {
  transform: translateZ(44px);
  }
}
@keyframes spinner-y0fdc1 {
 0% {
  transform: rotate(45deg) rotateX(-25deg) rotateY(25deg);
 }

 50% {
  transform: rotate(45deg) rotateX(-385deg) rotateY(25deg);
 }

 100% {
  transform: rotate(45deg) rotateX(-385deg) rotateY(385deg);
 }
}
.spinner > div {
 background-color: rgba(0,77,255,0.2);
 height: 100%;
 position: absolute;
 width: 100%;
 border: 2px solid #004dff;
}




.modal_open {
  display: flex;
  position: fixed;

}

.app1{
  display: grid;
  grid-template-columns: 15vw 80vw;
  flex-direction: column;
  min-height: fit-content;
  width: 100%;
  box-sizing: border-box;

}


h1{
  display: flex;
  justify-content: center;
  margin-bottom: 5px;
  
}
.book{
  display: grid;
  grid-template-columns: 30vw 10px 30vw;
  margin: 80px auto ;
  position: relative;
  
  @media ( max-width: 788px) {
    grid-template-columns: 55vw;
    border: 20px solid rgba(77, 58, 23);
    border-right: 10px solid #000000ae;
    border-top-right-radius: 50% 37px;
    border-bottom-right-radius: 50% 47px;
    animation: bendText 2s 1;
    
    .post-cont1::before {
      border: 0;
    }
    .post-cont2 h1{
      display: none;
    }
    .post-cont2{
      border-top: none;
      margin-top: -4rem;
      border-top-left-radius: 0;
      border-top-right-radius: 0;
      border-bottom-left-radius: 90% 25px;
        border-bottom-right-radius: 90% 40px;
    }
    .post-cont2::before {
      border: 0;
    }
    .sq{
      height: 0;
      appearance: none;
    }
    
  }
}  

@keyframes bendText {
0% {
  transform: rotate(-2deg);
}
100% {
  transform:  scalex(1);
}
}
.sq{
  background-color: #000000ae;
  position: relative;
  top:1.1rem;
  height: calc(100% - 2.37rem);
}
.dark-mode .sq{
  background-color: #ffffffae;
}
.post-cont1 {
  position: relative;
  border: 2px solid rgba(0, 0, 0, 0.395);
  border-top-left-radius: 90% 25px;
  border-top-right-radius: 90% 40px;
  border-bottom-left-radius: 90% 25px;
  border-bottom-right-radius: 90% 40px;
  padding: 50px 20px;
  box-shadow: -12px 4px 10px  #00000057;
  background-color: #ffffff;
  margin-top: -0.1rem;
  }

.post-cont1::before {
  content: '';
  position: absolute;
  top: -1.1rem;
  left: -2rem;
  width: 30.6vw;
  height: 100%;
  border: 20px solid rgba(77, 58, 23);
  border-right: 0;
  border-top-right-radius: 50% 37px;
  border-bottom-right-radius: 50% 47px;
  z-index: -1;
}
.post-cont2::before {
  content: '';
  position: absolute;
  top: 1.3rem;
  left: -0.1rem;
  width: 30.5vw;
  height: calc(100% - 2.8rem) ; 
  border: 17px solid rgba(77, 58, 23);
  border-left: 0;
  border-top-left-radius: 50% 27px;
  border-bottom-left-radius: 50% 38px;
  z-index: -1;
}



.post-cont2{
  margin-top: -23px;
  position: relative;
  border: 2px solid rgba(0, 0, 0, 0.49);
  border-top-left-radius: 40% 80px;
  border-top-right-radius: 90% 30px;
  border-bottom-left-radius: 90% 40px;
  border-bottom-right-radius: 90% 35px;
  padding: 50px 20px;
  box-shadow: 12px 4px 10px -6px #00000057;
  background-color: #ffffff;
  
}


.dark-mode {
  background-color: rgba(23, 23, 23);
  color: white;
  min-height: 100vh;
}
.dark-mode h3 {
  
  box-shadow: inset 2px -4px 10px #ffffff33;
}
.dark-mode button {
  box-shadow: inset 2px -4px 10px rgba(255, 255, 255, 0.2);
  color: #ffffff;
  background:#3a3a3a ;

}
.dark-mode .post-cont1{
  background-color: #57575733;
  box-shadow: -12px 4px 10px  #ffffff57;
  @media ( max-width: 788px) {
    background-color: #232323;
  }
}

.dark-mode .post-cont2{
  background-color: #57575733;
  @media ( max-width: 788px) {
    background-color: #232323;
  }
}
.dark-mode .post-cont .post {
  border: 2px solid rgba(255, 255, 255, 0.395);
  box-shadow: -12px 4px 10px -6px #ffffff2d;

}
 
</style>
