import { createRouter, createWebHistory } from 'vue-router';
import PostsPage from '@/Pages/Posts';
import UserPage from '@/Pages/User';

import LoginPage from '@/Pages/Login';
const routes = [
  {
    path: '/',
    component: PostsPage,
  },
  {
    path: '/user/:username',
    name: 'user',
    component: UserPage
  },
  {
    path: '/Login',
    component: LoginPage,
  },
  
];

const router = createRouter({
  routes,
  history: createWebHistory(process.env.BASE_URL),
});

export default router;
