<template>
    <div class="cmp" :class="{ 'dark_mode': isDarkMode }">
      <nav class="nav">
        <button class="nav-item1" @click="goTo('/')">
          <span class="nav-icon">&#128240;</span>
           Новости
        </button>
        <button class="nav-item1"  @click="goTo({ name: 'user', params: { username: 'me' } })">
          <span class="nav-icon">&#129503;</span>
          Мой профиль
        </button>
        <button class="nav-item1" @click="showEditProfile = true">
          <span class="nav-icon">&#128295;</span>
          Настройки
        </button>
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
        <button class="nav-item1" @click="changeDarkMode"><span class="nav-icon">&#127753;</span>Тема</button>
        <button v-if="!hasToken" class="nav-item1" @click="goTo('/Login')">
          <span class="nav-icon">&#128221;</span>
          Вход
        </button>
        <button v-else class="nav-item1" @click="exit">
          <span class="nav-icon">&#129340;</span>
          Выход
        </button>
      </nav>
    </div>
  </template>
  
  
<script>
import { mapState, mapMutations } from 'vuex';
import{ ElNotification} from 'element-plus' 
import EditProfile from './EditProfile.vue';
import axios from 'axios';
export default {
  name: "Menu1",
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
        avatarId: null
      },
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
    goTo(route) {
      this.$router.push(route);
    },
    exit(){
      localStorage.clear();
      this.$router.push('/')
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
.cmp{
  position: relative; 
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  background-color: #273840;
  height: 100%;
  z-index: 2;
  min-width: max-content;
  .nav{
    position: relative; 
    display: flex;
    flex-direction: column;
    padding-top: 100px;
    gap:15px;
    flex-grow: 1;
    width: max-content;
    margin: 0 auto;
  }
  button{
    display: flex;
    align-items: center;
  }
  .nav-item1{
    height: max-content;
    text-align: center;
    border-bottom: 1px dashed aliceblue;;
    text-decoration: none;
    color:aliceblue;
    display: flex;
    border-top:none;
    border-left:none; border-right:none;
    background: none;
    cursor: pointer;
    button{
      display: flex;
      text-align: center;
    }
  }
} 
.dark_mode input{
  color: #333;
}
.dark_mode.cmp {
  background-color:#72b1cf; 
}
.dark_mode .nav-item1{
  color: #333;
  border-bottom: 1px dashed #333;
}
.nav-icon {
  font-size: 20px;
  margin-right: 8px;
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
</style>