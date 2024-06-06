<template>
  <lamp></lamp>
  <Menu></Menu>
  <div class="app2" :class="{ 'dark-mode': isDarkMode }">
    <Menu1 class="menu"></Menu1>
    <div :class="{ 'user-info': user_info }" >
      <h1  v-if="user.id == my_id">My Profile<span class="edit_profile" @click="showEditProfileModal = true">&#9881;</span></h1>
      <h1  v-else>User's page: {{ $route.params.username }}</h1>
      <EditProfile :title="'Edit Profile'" v-model:visible="showEditProfileModal">
        <template #body>
          <div class="module1">
            <div class="container">
              <div class="card-inner__input">
                <br/>
                <input class="in_form" type="text" id="firstName" v-model="editProfileData.firstName" autofocus autocomplete="given-name" placeholder=" ">
                <span class="floating-label">FirstName</span>
                <span class="card-inner__input-icon">&#128209;</span>
              </div>
              <span v-if="editProfileErrors.firstName" class="error">{{ editProfileErrors.firstName }}</span>
            </div>
            <div class="container">
              <div class="card-inner__input">
                <br/>
                <input class="in_form" type="text" id="lastName" v-model="editProfileData.lastName" autocomplete="family-name" placeholder=" ">
                <span class="floating-label">LastName</span>
                <span class="card-inner__input-icon">&#128209;</span>
              </div>
            </div>
            <div class="container">
              <div class="card-inner__input">
                <br/>
                <input class="in_form" type="text" id="username" v-model="editProfileData.username" autocomplete="username" placeholder=" ">
                <span class="floating-label">Username</span>
                <span class="card-inner__input-icon">&#128273;</span>
              </div>
              <span v-if="editProfileErrors.username" class="error">{{ editProfileErrors.username }}</span>
            </div>
            <div class="container">
              <div class="card-inner__input">
                <br/>
                <input class="in_form" type="email" id="email" v-model="editProfileData.email" autocomplete="email" placeholder=" ">
                <span class="floating-label">Email</span>
                <span class="card-inner__input-icon">&#128231;</span>
              </div>
              <span v-if="editProfileErrors.email" class="error">{{ editProfileErrors.email }}</span>
            </div>
            <div class="container">
              <div class="card-inner__input">
                <br/>
                <input class="in_form" type="password" v-model="editProfileData.password" id="password" placeholder=" ">
                <span class="floating-label">Password</span>
                <span class="card-inner__input-icon">&#128274;</span>
              </div>
              <span v-if="editProfileErrors.password" class="error">{{ editProfileErrors.password }}</span>
            </div>
            <input class="in_photo" type="file" accept="image/*" @change="handleFileUpload">
          </div>
        </template>
        <template #btn>
          <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="updateProfile">Submit</button>
          <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="close('showEditProfileModal')">Close</button>
        </template>
      </EditProfile>
      <div v-if="user" class="user"  :style="avatarStyle">
        <h3 class="info"  v-if="user.lastName">{{ user.firstName }} {{ user.lastName }} -<br>- {{ user.username}}</h3>
        <h3 class="info" v-else>{{ user.firstName }} -<br>- {{ user.username}}</h3>
        <p class="info">{{ user.email }}</p>
        <p class="infoid">{{ user.id }}<br>UserId</p>
        <div class="user-btn">
          <div class="circle"></div>
          <input type="checkbox" class="curtain" v-model="user_info">
          <div class="grid">
            <div class="box"></div>
            <div class="box"></div>
            <div class="box"></div>
            <div class="box"></div>
            <div class="box"></div>
            <div class="box"></div>
            <div class="box"></div>
            <div class="box"></div>
            <div class="box"></div>
          </div>
        </div>
      </div>
      

      <div class="list">
        <label>
          <input type="radio" value="Posts" v-model="categories"> Posts
          <input type="radio" value="Photos" v-model="categories"> Photos
        </label>
        <div>
          <span class="btn_span" v-if="categories === 'Posts' && user.id == my_id " @click="showAddPostModal = true">&#10133;</span>
          <span class="btn_span" v-if="categories === 'Photos' && user.id == my_id" @click="showAddPhotoModal = true">&#10133;</span>
          <span>Выбрано: {{ categories }}</span>
          <AddPost :title="'Add Post'" v-model:visible="showAddPostModal">
            <template #body>
              <textarea rows="5" cols="33" style ="padding:10px;" v-model="newPostText" placeholder="Enter your post"></textarea>
            </template>  
            <template #btn>
              <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="addPost">Submit</button>
              <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="close('showAddPostModal')">Close</button>
            </template>
          </AddPost>
          <AddPhoto :title="'Add Photo'" v-model:visible="showAddPhotoModal">
            <template #body><input class="in_photo" type="file" accept="image/*" @change="handleFileUpload"></template>
            <template #btn>
              <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="updatePhoto">Submit</button>
              <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="close('showAddPhotoModal')">Close</button>
            </template>
          </AddPhoto>
        </div>
      </div>
        <div v-if="categories === 'Posts'" class="post-info">
          <div v-if="user.posts!=''">
            <div v-for="post in user.posts" :key="post.id" class="post">
              <div class="post-text">
                <div class="post-autor">
                  <span class="post-autor__icon">&#127183;</span>
                  
                  <span>{{ user.username }}</span>
                  <span class="formdata">{{formattedDate(post.created)}}</span>
                  <div  v-if="post.authorId== my_id || isAdmin==='true'" class="edit-icon">
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
                          <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="close('showEditPostModal')">Close</button>
                        </template>
                    </EditPost>
                    </div>
                  </div>
                </div>
                
                <h4>{{ post.text }}</h4>
              </div>
              <!-- <div class="post-title">
                  <div class="content">
                      <h3>{{ post.text }}</h3>
                      <span  v-if="user.id == my_id"  class="edit_post" @click="openEditPostModal(post.id, post.text)">&#128395;</span>
                  </div>
                  
                  <div v-if="user.id == my_id" class="delete_post" @click="deletePost(post.id)">
                      <span>&#10006;</span>
                  </div>
              </div> -->
              <div class="comments">
                <span>&#129513; Комментарии ({{ post.comments.length }})</span>
                <AddComment :title="'Add Comment'" v-model:visible="showModal"  :postId="selectedPostId">
                  <template #body>
                    <textarea rows="5" cols="33" style ="padding:10px;" v-model="commentText" placeholder="Enter your comment"></textarea>
                  </template>
                  <template #btn>
                    <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="addComment()">Submit</button>
                    <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="close('showModal')">Close</button>
                  </template>
                </AddComment>
                <details v-if="post.comments.length != 0" @click="openComments(post.id)">
                  <summary></summary>
                    <div v-for="comment in post.comments" :key="comment.id" class="commet">
                      <div class="commet-text">
                        <div class="commet-autor">
                          <span class="commet-autor__icon">&#129430;</span>
                          
                          <span v-if="comment.author">{{comment.author.username }}</span>
                          <span class="formdata">{{formattedDate(comment.created)}}</span>
                          <div v-if="comment.authorId== my_id || isAdmin==='true'" class="edit-icon">
                            <span>…</span>
                            <div class="edit-сomment">
                              <div @click="deleteComment(comment.id, post.id)">Удалить комментарий</div>
                              <div v-if="comment.authorId== my_id" @click="openEditCommentModal(post.id, comment.id, comment.text)">Редактировать комментарий</div>
                              <EditComment :title="'Edit Сommet'" v-model:visible="showEditCommentModal" :postId="selectedPostId" :commentId="selectedCommentId">
                                <template #body>
                                  <textarea rows="5" cols="33" style ="padding:10px;" v-model="editCommentText" placeholder="Enter your comment"></textarea>
                                </template>  
                              
                                <template #btn>
                                  <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="editComment">Submit</button>
                                  <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="close('showEditCommentModal')">Close</button>
                                </template>
                              </EditComment>
                            </div>
                          </div>
                        </div>
                        
                        <h5>{{ comment.text }}</h5>
                      </div>
                    </div>
                  </details>
                  <div v-if="this.my_id!=null" class="function_span"><span @click="openAddCommentModal(post.id)">+</span></div>
              </div>
            </div>
          </div>
          <div v-else>There are no posts</div>
        </div>
      <div v-if="categories === 'Photos'" class="albums">
        <Photos v-model:photos="user.photos"></Photos>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import EditPost from '@/components/EditPost.vue';
import EditComment from '@/components/EditComment.vue';
import EditProfile from '@/components/EditProfile.vue';
import Menu from '@/components/Menu.vue'
import Menu1 from '@/components/Menu1.vue'
import AddComment from '@/components/AddComment.vue';
import AddPost from '@/components/AddPost.vue'
import Lamp from '@/components/Lamp.vue';
import Photos from '@/components/Photos.vue';
import AddPhoto from '@/components/AddPhoto.vue'
import{ ElNotification} from 'element-plus' 

export default {
  components: {
    EditProfile,
    EditComment,
    EditPost,
    AddPost,
    Menu,
    Menu1,
    AddComment,
    Lamp,
    Photos,
    AddPhoto,
  },
  data() {
    return {
      isOpen: false,
      my_id:null,
      user: {},
      categories: 'Posts',
      errorMessage:{},
      showAddPostModal: false,
      showAddPhotoModal: false,
      showPhoto: false,
      showEditCommentModal:false,
      showEditPostModal: false,
      selectedPostId: null,
      selectedCommentId:null,
      showEditProfileModal: false,
      editProfileData: {
        firstName: '',
        lastName: '',
        username: '',
        email: '',
        password: '',
        avatarId: null
      },
      isAdmin:null,
      editProfileErrors: {},
      user_info:false,
      showModal: false,
    };
  },
  watch: {
    '$route.params.username': function() {
      this.fetchUser();
    }
  },
  computed: {
    isDarkMode() {
      return this.$store.state.isDarkMode;
    },
    avatarStyle() {
      if (!this.user_info){
        if (this.user&&this.user.avatar && this.user.avatar.url) {
          const avatarUrl = `https://localhost:7297${this.user.avatar.url}`;
          return {
            backgroundColor: 'transparent', 
            backgroundImage: `url(${avatarUrl})`,
            backgroundSize: 'cover',
            backgroundPosition: 'center',
          };
        } else {
          return {
            backgroundColor: 'rgba(163, 163, 163, 0.965)',
          };
        }
      }

    },
    
  },

  async mounted() {
    this.isAdmin = localStorage.getItem('isAdmin');
    this.my_id=localStorage.getItem('userId');
    const token = localStorage.getItem('token');
    if (!token && this.$route.params.username =='me') {
      this.$router.push('/Login')
      this.user = {}; 
      this.my_id = null; 
      this.isAdmin = null;
     
    }
    else(
      await this.fetchUser()
    )
  },
  methods: {
    async openComments(postId) {
      const postToUpdate = this.user.posts.find(p => p.id === postId); 

      if (postToUpdate) {
        const response = await axios.get(`https://localhost:7297/api/posts/${postId}`);

        if (response.data) {
          postToUpdate.comments = response.data.comments;
        }
      }
    },
    async beforeRouteUpdate(to, from, next) {
      await this.fetchUserData();
      next();
    },
    validateProfile() {
      this.editProfileErrors = {};
      if (!this.editProfileData.firstName) {
        this.editProfileErrors.firstName = 'First name is required.';
      }
      if (!this.editProfileData.username) {
        this.editProfileErrors.username = 'Username is required.';
      }
      if (!this.validateEmail(this.editProfileData.email)) {
        this.editProfileErrors.email = 'Invalid email address.';
      }
      if (!this.editProfileData.password) {
        this.editProfileErrors.password = 'Password is required.';
      } else if (this.editProfileData.password.length < 8 || this.editProfileData.password.length > 20) {
        this.editProfileErrors.password = 'Password must be between 8 and 20 characters.';
      }
      return Object.keys(this.editProfileErrors).length === 0;
    },
    validateEmail(email) {
      var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@(([^<>()[\]\\.,;:\s@"]+\.[^<>()[\]\\.,;:\s@"]{2,}))$/i;
      return re.test(email.toLowerCase());
    },
    
    handleFileUpload(event) {
      this.selectedFile = event.target.files[0];
    },
    async updatePhoto() {
      if (this.selectedFile) {
        try {
          const formData = new FormData();
          formData.append('file', this.selectedFile);
          formData.append('description', 'Users photo');
          const photoResponse = await axios.post('https://localhost:7297/api/photos', formData, {
            headers: {
              'Content-Type': 'multipart/form-data',
              Authorization: `Bearer ${localStorage.getItem('token')}`
            }
          });
          this.showAddPhotoModal = false;
          this.selectedFile = null;
          this.handlePhotoAdded(photoResponse.data);
        } catch (error) {
          this.errorMessage = error.response ? error.response.data.message : 'Error adding photo';
          ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
        }
      }
    },
    
    handlePhotoAdded(newPhoto) {
      debugger;
      this.user.photos.push({
        ...newPhoto,
        url: `https://localhost:7297${newPhoto.url}`

      });
    },
    async updateProfile() {
      if (this.validateProfile()) {
        try {
          let avatarId = this.editProfileData.avatarId;

          if (this.selectedFile) {
            const formData = new FormData();
            formData.append('file', this.selectedFile);
            formData.append('description', 'User avatar');
            const photoResponse = await axios.post('https://localhost:7297/api/photos', formData);
            avatarId = photoResponse.data.id;
          }

          const updateData = {
            ...this.editProfileData,
            avatarId: avatarId
          };

          const response = await axios.put('https://localhost:7297/api/users/me', updateData);
          this.showEditProfileModal = false;
          this.user = response.data;

          this.showEditProfileModal = false;

          await this.fetchUser();
        } catch (error) {
          this.errorMessage = error.response ? error.response.data.message : 'Error updating profile';
          ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
        }
      }
    },
    async fetchUser() {
      try {
        const username = this.$route.params.username;
        const response = await axios.get(`https://localhost:7297/api/users/${username}`);
        this.user = response.data;
        this.editProfileData = {
          firstName: this.user.firstName,
          lastName: this.user.lastName,
          username: this.user.username,
          email: this.user.email,
          password: '',
          avatarId: this.user.avatar ? this.user.avatar.id : null
        };
      } catch (error) {
        this.errorMessage = error.response ? error.response.data.message : 'Error fetching user data';
        ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
      }
    },
    openEditCommentModal(postId, commentId ,commentText) {
      this.selectedPostId = postId;
      this.selectedCommentId = commentId;
      this.editCommentText = commentText;
      this.showEditCommentModal = true;
    },
    openEditPostModal(postId, postText) {
      this.selectedPostId = postId;
      this.editPostText = postText;
      this.showEditPostModal = true;
    },
    close(modalName) {
      this[modalName] = false;
    },
    async editComment(){
      if (!this.selectedCommentId) return;
      try {
       
        await axios.put(`https://localhost:7297/api/comment/${this.selectedCommentId}`, {
          text: this.editCommentText
        });
        this.showEditCommentModal = false;
        this.handleCommentEdited(this.selectedPostId,this.selectedCommentId, this.editPostText);
        this.editCommentText = ''; 
      } catch (error) {
        this.errorMessage = error.response ? error.response.data.message : 'Error editing commentt';
        ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
      }
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
    handleCommentEdited(postId, commentId, updatedText, userContext) { 
      const postIndex = userContext.user.posts.findIndex(post => post.id === postId);
      if (postIndex !== -1) {
        const commentIndex = userContext.user.posts[postIndex].comments.findIndex(comment => comment.id === commentId);
        if (commentIndex !== -1) {
          userContext.user.posts[postIndex].comments[commentIndex].text = updatedText;
        }
      }
    },
    handlePostEdited(postId, updatedText) {
      const postIndex = this.user.posts.findIndex(post => post.id === postId);
      if (postIndex !== -1) {
        this.user.posts[postIndex].text = updatedText;
      }
    },
    async addPost() {
      try {
        const response = await axios.post('https://localhost:7297/api/posts', {
          text: this.newPostText
        });
        this.showAddPostModal = false;
        this.newPostText = ''; 
        this.handlePostAdded(response.data);
      } catch (error) {
        this.errorMessage = error.response ? error.response.data.message : 'Error adding post';
        ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
      }
    },
    handlePostAdded(newPost) {
      this.user.posts.push(newPost);
    },
    async addComment() {
      try {
        const response = await axios.post(`https://localhost:7297/api/comment/${this.selectedPostId}`, {
          text: this.commentText
        });
        this.showModal = false;
        this.commentText = ''; 
        this.handleCommentAdded(response.data);
      } catch (error) {
        this.errorMessage = error.response.data.errors ? error.message : 'Error adding comment';
        ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
      }
    },
    async deleteComment(CommentId,PostId){
      try {
        await axios.delete(`https://localhost:7297/api/comment/${CommentId}`);
        this.handleCommentDeleted(CommentId,PostId);
      } catch (error) {
        this.errorMessage = error.response ? error.response.data.message : 'Error deleting comment';
        ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
      }
    },
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
      this.user.posts = this.user.posts.filter(post => post.id !== postId);
    },
    handleCommentDeleted(CommentId, PostId) {
      const postIndex = this.user.posts.findIndex(post => post.id === PostId);

      if (postIndex !== -1) { 
        this.user.posts[postIndex].comments = 
          this.user.posts[postIndex].comments.filter(comment => comment.id !== CommentId);
      }
    },
    handleCommentAdded(newComment) {
      const postIndex = this.user.posts.findIndex(post => post.id === this.selectedPostId);
      if (postIndex !== -1) {
        this.user.posts[postIndex].comments.push(newComment);
      }
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
    openAddCommentModal(postId) {
      this.selectedPostId = postId;
      this.showModal = true;
    },

  },
  
};
</script>
<style lang="scss" scoped>
.formdata{
  padding-left: 2px;
}
/// Редактирование постов
.post-info{
  display: flex;
  flex-direction: column;
  margin: 10px auto 0 auto;
  width: 64vw;
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
          
          .edit-сomment{
            flex-direction: column;
            display: none;
            gap:5px;
            padding: 5px;
            border-radius:10px;
            border: 2px solid #a4a2a23c;
            position: absolute;
            margin-top: 60px;
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
            .edit-сomment{
              display: flex;
              cursor: pointer;
            }
          }
        }
        h5{
          width: 90%;
          margin: 15px auto 5px auto;
        }
      }
    }
      
      summary{
        margin: 3px 5px 0 ;
      }
    }
  }
}

//общие
.app2{
  display: grid;
  grid-template-columns: 15vw 80vw;
  padding-top: 10vh;
  .btn_span{
    cursor: pointer;
  }
  .menu{
    height: calc(100% + 10vh);
    margin-top:-10vh; 
  }
  h1{
    display: flex;
    justify-content: center;
    text-align: center;
    margin-bottom: 5px;
    grid-column: span 2; 
  }
 
  .edit_profile{
    margin-left: 10px;
    margin-top: -2px;
  }
  .list{
    position: relative;
    display: flex;
    flex-direction: column;
    margin-top: 15vh;
    align-items: center;
  }
  .user-info{
    .grid {
      background-color: #2e434d;
    } 
    .info{
      color: #273840;
      animation: drawFromBottom 1.5s ease-out; 
    }
    .infoid{
      color: #273840;
      animation: drawFromBottom 1.5s ease-out; 
    }
     .user{
      background-color: #f8f3f3;
      
    }
    .box {
      border-radius: 50%;
      background: linear-gradient(145deg, #68bce6, #7be0ff);
      box-shadow: 20px 20px 60px #5192b3, -20px -20px 60px #96ffff;
    } 
    .circle {
      opacity: 0;
    }
  } 
}
.dark-mode.app2{
  background-color: rgba(23, 23, 23);
  color:white;
  .post-info .post .post-text .post-autor .edit-post{
    background-color: rgb(92, 91, 91);
  }
  .post-info .post .comments .commet .commet-text .commet-autor .edit-сomment{
    background-color: rgb(92, 91, 91);
  }
  
   
  .edit-сomment{
    background-color:rgb(92, 91, 91)
  }
 
}
@keyframes drawFromBottom {
  from {
    transform: translateY(100%);
  }
  to {
    transform: translateY(0); 
  }
}

// Редактирование профиля
.module1{
  display: flex;
  margin: 0 auto;
  background-color: #273840;
  justify-content: space-around;
  flex-direction: column;
  align-items: center;
  height: clamp(400px,70vh,1000px);
  box-shadow: 0 0 10px rgba(0,0,0,0.25);
  border-radius: 8px;
  position: relative;
  backdrop-filter: blur(10px);
  .in_photo{
    width: 80%;
    color: #06c31c;
  }
  .in_photo::file-selector-button {
      border: 2px solid #7c7c7c;
      color: #06c31c;
      border-radius: 5px;
      background-color: #455c66;
      cursor: pointer;
      font-size: 12px;
      box-sizing: border-box;
      width: 40%;
      padding: 1px;
      transition: background .2s ease-in-out;
  }
  .in_photo::file-selector-button:hover {
    background: #aaaaaa7e;
  }
  .container{
    display: flex;
    flex-direction: column;
    width: 80%;
    .error{
        display: flex;
        text-align: start;
        color: red;
        font-size: 10px;
    }
    .card-inner__input{
      border: 2px solid #7c7c7c;
      border-radius: 5px;
      display: flex;
      align-self: center;
      width: 100%;
      &-icon{
          display: flex;
          width: 15%;
          justify-content: center;
          align-self: center;
      }
      .in_form{
        width: 85%;
        border: 0;
        background-color: transparent;
        font-size: 16px;
        padding: 4px;
        color:#06c31c;
        outline: 0;
      }
      .floating-label {
        position: absolute;
        left: 13%;
        padding-top: 2%;
        color:#06c31c;
        pointer-events: none;
        font-size: 16px;
        justify-content: flex-start;
        transition: 0.2s ease all;
      }
      .in_form:focus ~ .floating-label,
      .in_form:not(:placeholder-shown)~ .floating-label{
        font-size: 10px;
        padding: 0;
        margin-top: -7px;
        background-color: #273840;
        height: max-content;
      }
    }
    .card-inner__input:focus-within {
        outline: 3px solid #06c31c;
        border-radius: 5px;
        border: none;

    }
  } 
}


//данные пользователя
.user {
  width: 60vw;
  height: 40vh;
  border-radius: 20px;
  border: 2px solid #273840;
  display: flex;
  flex-direction: column;
  margin: 2rem auto;
  padding: 30px 20px 0 20px;
  box-shadow: 4px 4px 10px #1a1a1ab8, -4px -4px 10px #2f2f2fb8;
  animation: slideIn 1.5s ease-in-out;
 
  .info{
    color: transparent
  }
  .infoid{
    width: 15vh;
    position: relative;
    bottom: 0;
    color: transparent;
  }
  h3 {
  text-align: center;
  }
  p {
    text-align: center;
    margin: 1rem 0;
  }
  .user-btn{
    position: relative;
    width: 13vh;
    height: 13vh;
    border-radius: 23px;
    background: linear-gradient(145deg, #f0f0f0, #cacaca);
    box-shadow: 4px 4px 10px #9d9d9d, -4px -4px 10px #ffffff;
    left: calc((100% - 13vh)/2);
    top: 10%;
    
    .circle{
      border-radius: 50%;
      border: 0.3em dashed #5cd6ff;
      height: 5vh;
      width: 5vh;
      margin: auto;
      position: absolute;
      left: 0;
      right: 0;
      top: 0;
      bottom: 0;
      z-index: 1;
      animation: moveLine 4s infinite linear;
    }
    .curtain{
      position: relative;
      width: 13vh;
      height: 13vh;
      appearance: none;
      z-index: 1;
    }
    .grid {
      border-radius: 25%;
      padding: 2vh;
      width: 10vh;
      height: 10vh;
      background: #37505c;
      display: grid;
      grid-template-columns: auto auto auto;
      align-items: center;
      position: absolute;
      overflow: hidden;
      left: 0;
      right: 0;
      top: 0;
      bottom: 0;
      margin: auto;
      .box {
        position: relative;
        margin: auto;
        left: 0;
        right: 0;
        top: 0;
        bottom: 0;
        width: 10px;
        height: 10px;
        border-radius: 50%;
        background: linear-gradient(145deg, #3b5662, #324853);
        box-shadow: 20px 20px 60px #273840, -20px -20px 60px #486878;
        overflow: hidden;
      }
    }
  }
}
.dark-mode .user {
  border: 2px solid #70a0b6;
  box-shadow: 4px 4px 10px #dfdfdfb8, -4px -4px 10px #cececeb8;
}
@keyframes slideIn {
  from {
    transform: translateY(100%);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}
@keyframes moveLine {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg);
  }
}
</style> 