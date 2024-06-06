<template>
    
    <div class="logpage" :class="{ 'dark__mode': isDarkMode }">
        <Menu></Menu>
        <lamp></lamp>
        <Menu1></Menu1>
        <div class="card-inner" v-if="!registrationbool">
            <h1>Вход</h1>
            <div class="container">
                <div class="card-inner__input">
                    <br/>
                    <input class="in_form" type="text" id="login" v-model="loginData.username" autofocus autocomplete="login" placeholder=" ">
                    <span class="floating-label">Никнейм</span>
                    <span class="card-inner__input-icon">&#128273;</span>
                </div>
                <span v-if="loginErrors.username" class="error">{{ loginErrors.username }}</span>
            </div>
            <div class="container">
                <div class="card-inner__input">
                    <br/>
                    <input class="in_form" type="password" v-model="loginData.password" id="password" placeholder=" ">
                    <span class="floating-label">Пароль</span>
                    <span class="card-inner__input-icon">&#128274;</span>
                </div>
                <span v-if="loginErrors.password" class="error">{{ loginErrors.password }}</span>
            </div>
            <button class="card-inner__btn" @click="logIn">Вход</button>
            <button class="card-inner__btn" @click="registration">Регистрация</button>
        </div>
        <div class="card-inner" v-else>
            <h1>Регистрация</h1>
            <div class="container">
                <div class="card-inner__input">
                    <br/>
                    <input class="in_form" type="text" id="firstName" v-model="registrationData.firstName" autofocus autocomplete="login" placeholder=" ">
                    <span class="floating-label">Имя</span>
                    <span class="card-inner__input-icon">&#128209;</span>
                </div>
                <span v-if="registrationErrors.firstName" class="error">{{ registrationErrors.firstName }}</span>
            </div>
            <div class="container">
                <div class="card-inner__input">
                    <br/>
                    <input class="in_form" type="lastName" v-model="registrationData.lastName" id="lastName" placeholder=" ">
                    <span class="floating-label">Фамилия</span>
                    <span class="card-inner__input-icon">&#128209;</span>
                </div>
            </div>
            <div class="container">
                <div class="card-inner__input">
                    <br/>
                    <input class="in_form" type="text" id="login" v-model="registrationData.username"  autocomplete="login" placeholder=" ">
                    <span class="floating-label">Никнейм</span>
                    <span class="card-inner__input-icon">&#128273;</span>
                </div>
                <span v-if="registrationErrors.username" class="error">{{ registrationErrors.username }}</span>
            </div>
            <div class="container">
                <div class="card-inner__input ">
                    <br/>
                    <input class="in_form" type="email" id="email" v-model="registrationData.email"  autocomplete="email" placeholder=" ">
                    <span class="floating-label">Почта</span>
                    <span class="card-inner__input-icon">&#128231;</span>
                </div>
                <span v-if="registrationErrors.email" class="error">{{ registrationErrors.email }}</span>
            </div>
            <div class="container">
                <div class="card-inner__input">
                    <br/>
                    <input class="in_form" type="password" v-model="registrationData.password" id="password" placeholder=" ">
                    <span class="floating-label">Пароль</span>
                    <span class="card-inner__input-icon">&#128274;</span>
                </div>
                <span v-if="registrationErrors.password" class="error">{{ registrationErrors.password }}</span>
            </div>
            <input class="in_photo" type="file" accept="image/*"  @change="handleFileUpload">
            <button class="card-inner__btn" @click="registrationid">Регистрация</button>
            <button class="card-inner__btn" @click="log">Вход</button>  
        </div>
        <div class="round round1"></div>
        <div class="round round2"></div>
    </div>
</template>
<script>
import axios from 'axios';  
import Menu from '@/components/Menu.vue'
import Menu1 from '@/components/Menu1.vue'
import{ ElNotification} from 'element-plus' 
import 'element-plus/dist/index.css';
import Lamp from '@/components/Lamp.vue';
export default {
    data() {
        return {
            loginData: {
                username: '',
                password: ''
            },
            registrationbool: false,
            registrationData: {
                firstName: '',
                lastName: '',
                username: '',
                email: '',
                password: '',
                avatarId: null
            },
            errorMessage: '',
            loginErrors: {},
            registrationErrors: {},
            registrationbool: false,
            selectedFile:null,
        };
    },
    components: {
        Menu,
        Menu1,
        Lamp
        
    },
    computed: {
        isDarkMode() {
            return this.$store.state.isDarkMode;
        },
    },

    methods: {
        handleFileUpload(event) {
            this.selectedFile = event.target.files[0];
        },
        validateEmail(email) {
            const re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@(([^<>()[\]\.,;:\s@"]+\.)+[^<>()[\]\.,;:\s@"]{2,})$/i;
            return re.test(String(email).toLowerCase());
        },

        validateLogin() {
            this.loginErrors = {};
            if (!this.loginData.username) {
                this.loginErrors.username = 'Имя пользователя обязательно.';
            }
            if (!this.loginData.password) {
                this.loginErrors.password = 'Пароль обязателен.';
            }
            return Object.keys(this.loginErrors).length === 0;
        },
        validateRegistration() {
            this.registrationErrors = {};
            if (!this.registrationData.firstName) {
                this.registrationErrors.firstName = 'Имя обязательно.';
            }
            if (!this.registrationData.username) {
                this.registrationErrors.username = 'Имя пользователя обязательно.';
            }
            if ( !this.validateEmail(this.registrationData.email)) {
                this.registrationErrors.email = 'Неправильно введен e-mail.';
            }
            if (!this.registrationData.password) {
                this.registrationErrors.password = 'Пароль обязателен.';
            } else if (this.registrationData.password.length < 8 || this.registrationData.password.length > 20) {
                this.registrationErrors.password = 'Пароль должен быть от 8 до 20 символов.';
            }
            return Object.keys(this.registrationErrors).length === 0;
        },
        async registrationid(){
            if (this.validateRegistration()) {
                try {
                    const response = await axios.post('https://localhost:7297/api/auth/signup', this.registrationData);
                    localStorage.clear;
                    localStorage.setItem('token', response.data.token);
                    
                    axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.token}`;
                    
                    
                    if (this.selectedFile) {
                        const formData = new FormData();
                        formData.append('file', this.selectedFile);
                        formData.append('description', 'User avatar');
                        const photoResponse = await axios.post('https://localhost:7297/api/photos', formData, {
                            headers: {
                                'Content-Type': 'multipart/form-data',
                            },
                        });

                    const updateData = {
                        ...this.registrationData,
                        avatarId: photoResponse.data.id
                    };
                     await axios.put('https://localhost:7297/api/users/me', updateData);
                    const responseID = await axios.get('https://localhost:7297/api/users/me');
                    localStorage.setItem('userId', responseID.data.id);
                    localStorage.setItem('isAdmin', responseID.data.isAdmin);
                    this.$router.push({ name: 'user', params: { username: 'me' } });

                }

                
                } catch (error) {
                    if ( error.response.data.errors!=undefined) {
                        this.errorMessage = error.response.data.errors;
                    } 
                    else{
                        this.errorMessage = error.response.data.message
                    }
                    
                    ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
                }
            }
        },
        log(){
            this.registrationbool = false
        },
        registration(){
            this.registrationbool = true
        },
        async logIn() {
            if (this.validateLogin()){
                try {
                const response = await axios.post('https://localhost:7297/api/auth/login', this.loginData);
                localStorage.setItem('token', response.data.token);
                axios.defaults.headers.common['Authorization'] = `Bearer ${response.data.token}`;
                const responseID = await axios.get('https://localhost:7297/api/users/me');
                localStorage.setItem('userId', responseID.data.id);
                localStorage.setItem('isAdmin', responseID.data.isAdmin);
                this.$router.push({ name: 'user', params: { username: 'me' } });

                } catch (error) {
                    if ( error.response.data.errors!=undefined) {
                        this.errorMessage = error.response.data.errors;
                    } else {
                        this.errorMessage = error.response.data.message;
                    }
                    ElNotification({
                        title: 'Error',
                        message: this.errorMessage,
                        type: 'error',
                    });
                }
            }
            
        }
        

    }
}
</script>
<style lang="scss" scoped>
.in_photo{
    width: 80%;
    color: #06c31c;
}
.in_photo::file-selector-button {
border: 2px solid #7c7c7c;
  color: #06c31c;
  border-radius: 5px;
  background-color: #fff;
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
.dark__mode {
    background-color: rgba(23, 23, 23);
}
.logpage{
    display: grid;
    height: 100vh;
    grid-template-columns: 20vw 80vw;
    box-sizing: border-box;
    
    h1{
        color:#06c31c;
        text-shadow: 0 0 1px rgba(0,0,0,1);
    }
    .card-inner{
        display: flex;
        margin: 0 auto;
        transform: translatey(20vh);
        justify-content: space-around;
        flex-direction: column;
        align-items: center;
        width: clamp(200px,25vw,400px);
        height: clamp(400px,70vh,1000px);
        box-shadow: 0 0 10px rgba(0,0,0,0.25);
        border-radius: 8px;
        position: relative;
        z-index: 2;
        backdrop-filter: blur(10px);
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
            }
            .card-inner__input:focus-within {
                outline: 3px solid #06c31c;
                border-radius: 5px;
                border: none;
            
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
            background-color: rgb(255, 255, 255);
            height: max-content;
        }
        &__btn{
            width: 80%;
            height: 7%;
            border-radius:5px ;
            background-color: #06c31c;
            border: 1px solid #7c7c7c;
            color:white
        }
        
    }
    .round{
        background-color: #06c31c;
        width: clamp(100px,12vw,100px);
        height:  clamp(100px,12vw,100px);
        border-radius: 50%;
        position: absolute;
            z-index: 1;
    }
    .round1{
        top:15%; 
        right: 48%;
        animation: flyingcircle 2s infinite alternate;
    }
    @keyframes flyingcircle{
        0% {
            transform: scale(0.8)translateY(-20px)
        }
        100% {
            transform:  scale(1)translateY(20px)
        }
    }
    .round2{
        bottom:7%;
        left:69%;
        animation: flyingcircle1 2s infinite alternate;
    }
    @keyframes flyingcircle1{
        0% {
            transform: scale(1)translateY(20px);
        }
        100% {
            transform:  scale(0.8)translateY(-20px);
        }
    }
    
   
}
.logpage.dark__mode .in_form:-webkit-autofill,
.logpage.dark__mode .in_form:-webkit-autofill:hover,
.logpage.dark__mode .in_form:-webkit-autofill:focus,
.logpage.dark__mode .in_form:-webkit-autofill:active {
    -webkit-box-shadow: 0 0 0 30px rgb(47, 47, 47) inset !important; 
    -webkit-text-fill-color: #06c31c !important; 
}
.logpage .in_form:-webkit-autofill,
.logpage .in_form:-webkit-autofill:hover,
.logpage .in_form:-webkit-autofill:focus,
.logpage .in_formput:-webkit-autofill:active {
    -webkit-box-shadow: 0 0 0 30px rgb(255, 255, 255) inset !important; 
    -webkit-text-fill-color: #06c31c !important; 
}
.logpage.dark__mode .in_form:focus~ .floating-label,
.logpage.dark__mode  .in_form:not(:placeholder-shown) ~ .floating-label {
    background-color: rgb(47, 47, 47); 
    }
</style>