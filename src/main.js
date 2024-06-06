import { createApp } from 'vue';
import App from '@/App.vue';
import router from '@/router/router'
import store from '@/store/store';
import axios from 'axios';
import 'element-plus/dist/index.css'; 

const app = createApp(App);

const token = localStorage.getItem('token');
if (token) {
  axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  console.log('sus');
  fetchUserId();
}
async function fetchUserId() {
  try {
    const response = await axios.get('https://localhost:7297/api/users/me');
    localStorage.setItem('userId', response.data.id);
    localStorage.setItem('isAdmin', response.data.isAdmin);
    console.log(response.data.id)
  } catch (error) {
    console.error('Ошибка получения ID пользователя:', error);
  }
}
// components.forEach(component => {
//     app.component(component.name, component)
// })

app.use(router)
   .use(store)
   .mount('#app');
