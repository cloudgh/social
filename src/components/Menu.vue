<template>
  <div class="cmp" :class="{ 'dark_mode': isDarkMode }">
      <button class="nav-item1" @click="goTo('/')">
        <span class="nav-icon">&#128240;</span>
         Новости
      </button>
      <div class="nav-item3">
        <span>&#128269;</span>
        <input type="text" placeholder="Поиск" v-model="searchQuery" @keyup.enter="search">
        <span @click="clearer">&#10006;</span>
      </div>
      <div class="sarchResult" v-if="searchQuery!=''">
        <span v-if="searchResults.posts!=''">
          Posts:
          <div v-for="post in searchResults.posts" class='posts' > 
            <router-link :to="{ name: 'user', params: { username: post.author.username } }">
              <span :style="{ maxHeight: '30px', overflow: 'hidden' }">{{ post.text }}</span>
              <br>
              <span v-if="post.author">(Автор: {{ post.author.firstName }} {{ post.author.lastName }})</span>
            </router-link>
            
          </div>
        </span>
        <span v-if="searchResults.users!=''">
          User:
          <div v-for="user in searchResults.users" class="users"> 
            <router-link :to="{ name: 'user', params: { username: user.username } }">
              <span>&#129528;{{ user.firstName }} ({{ user.username }})</span>
            </router-link>
          </div>
        </span>
        <span style ="margin-bottom: 10px"  v-else >Ничего не нашлось</span>
      </div>
      <div class="nav-item2" @click="me" >
        <button class="btn"><span class="nav-icon">&#129503;</span>Мой профиль</button>
      <div class="list">
        <div class="nav-item1" @click.stop="showEditProfile = true" >&#128295;Настройки</div>
        <EditProfile :title="'Edit Profile'" v-model:visible="showEditProfile">
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
            <button style ="background-color: #06c31c; border: 1px solid #7c7c7c; color:#273840; border-radius:5px ; padding: 2px" @click="showEditProfile = false">Close</button>
          </template>
        </EditProfile>
        <button class="nav-item1" @click.stop="changeDarkMode">&#127753;Тема</button>
        <button v-if="!hasToken" class="nav-item1" @click.stop="goTo('/Login')">
          <span class="nav-icon">&#128221;</span>
          Вход
        </button>
        <button v-else class="nav-item1" @click.stop="exit">
          <span class="nav-icon">&#129340;</span>
          Выход
        </button>
      </div>
    </div>
  </div>
</template>


<script>
import { mapState, mapMutations } from 'vuex';
import EditProfile from './EditProfile.vue';
import{ ElNotification} from 'element-plus' 

import axios from 'axios';
export default {
  name: "Menu",
  data() {
    return {
      token: null,
      navigator:false,
      showEditProfile:false,
      editProfileData: {
        firstName: '',
        lastName: '',
        username: '',
        email: '',
        password: '',
        avatarId: null,
      },
      searchQuery:'',
      searchResults:[],
      editProfileErrors: {},
    };
  },
  components:{
    EditProfile
  },
  methods: {
    ...mapMutations(['updateDarkMode']),
    changeDarkMode() {
      this.updateDarkMode(!this.isDarkMode);
    },
    search() {
      if (this.searchQuery) {
        axios.get('https://localhost:7297/api/search', { params: { query: this.searchQuery } })
          .then(response => {
            this.searchResults = response.data;
          })
      }
    },
    clearSearch() {
      this.searchQuery = '';
      this.searchResults = null;
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
    me(){
      this.$router.push({ name: 'user', params: { username: 'me' } })
    },
    goTo(route) {
      this.$router.push(route);
    },
    clearer(){
      this.searchQuery = ""
    },
    exit(){
      localStorage.clear();
      this.$router.push('/')
    },
    handleFileUpload(event) {
      this.selectedFile = event.target.files[0];
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
        const response = await axios.get('https://localhost:7297/api/users/me');
        this.editProfileData = {
          firstName: response.data.firstName,
          lastName: response.data.lastName,
          username: response.data.username,
          email: response.data.email,
          password: '',
          avatarId: response.data.avatar ? response.data.avatar.id : null
        };
      } catch (error) {
        this.errorMessage = error.response ? error.response.data.message : 'Error fetching user data';
      }
    },
  },
  computed: {
   
    ...mapState(['isDarkMode']),
  
    hasToken() {
      return !!localStorage.getItem('token');
    }
  },
  async mounted() {
    
    this.my_id=localStorage.getItem('userId');
    const token = localStorage.getItem('token');
    if (!token && this.$route.params.username =='me') {
      this.$router.push('/Login')
    }
    else(
      await this.fetchUser()
    )
  },
}

</script>

<style scoped lang="scss">
* {
  box-sizing: border-box;
}
.dark_mode .users {
  a{
    color: #fff;
 
  }
}
.dark_mode .posts {
  a{
    color: #fff;
 
  }
}
.users{
  text-align: justify;
  padding: 5px;
  border: 2px solid #a4a2a23c;
  border-radius: 20px;
  cursor:pointer;
  margin-bottom: 10px;
  a{
    text-decoration: none;
    color: #171717;
  }
}
.posts{
  text-align: justify;
  background-color: #dcdcdc3a;
  border: 2px solid #a4a2a23c;
  border-radius: 20px;
  padding: 5px;
  margin-bottom: 10px;
  cursor:pointer;
  a{
    text-decoration: none;
    color: #171717;
  }
}
.dark_mode .sarchResult{
  background-color: rgb(23,23,23);
  color: #fff;
}
.sarchResult{
  position: absolute;
  display: flex;
  flex-direction: column;
  top:30px;
  left: 140px;
  max-width: 30vw;
  background-color: #fff;
  padding: 15px 15px 0 15px ;
  border-radius:20px ;
  border:2px solid #455c66;
  max-height: 50vh;
  overflow-y: auto;
}
.cmp{
  display: grid;
  grid-template-columns:10vw 20vw 1fr;
  position: fixed;
  z-index: 3;
  width: 100vw;
  padding:  5px 40px 5px 20px;
  border-bottom:2px solid #333 ;
  background-color: #fff;
  .btn{
    background-color: transparent;
    height: max-content;
    text-align: center;
    text-decoration: none;
    color: #333;
    border: none;
    cursor: pointer;
    
  }
  button{
    display: flex;
    align-items: center;
  }
  .nav-item3{
    display: flex;
    margin-left: 10px;
    padding: 0 4px ;
    border: 2px solid #333;
    border-radius: 20px;
    color: #333;
    height: max-content;
    cursor: pointer;
    input{
      border: none;
      outline: none;
      width: 80%;
      padding: 2px 0;
      background-color: transparent;
    }
    span{
      width: 10%;
    }
  }
  .nav-item2:hover{
    .list{
      display: flex;
      cursor: pointer;
    }
  }
  .nav-item2{
    display: flex;
    justify-self: end;
    flex-direction: column;
    .list{
      flex-direction: column;
      display: none;
      position: absolute;
      margin-top: 24px;
      justify-self: center;
      align-self: center;
      background-color: #fff;
    }
  }
  .nav-item1{
    height: max-content;
    text-align: center;
    text-decoration: none;
    color: #333;
    display: flex;
    border: none;
    background: none;
    cursor: pointer;
    button{
      display: flex;
      text-align: center;
    }
  }
} 
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
.dark_mode.cmp .nav-item2 .list{
  background-color: rgba(23,23,23);
}
.dark_mode input{
  color: #dddddd;
}
.dark_mode.cmp {
  background-color:#171717;
  border-bottom: 2px solid #dddddd;
}
.dark_mode .nav-item1{
  color: #dddddd;
}
.dark_mode .btn{
  color: #dddddd;
} 
.dark_mode .nav-item3{
  color: #dddddd;
}
.nav-icon {
  font-size: 20px;
}
</style>