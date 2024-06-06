import { createStore } from 'vuex';

export default createStore({
  state: {
    isDarkMode: false,
  },
  mutations: {
    updateDarkMode(state, newValue) {
      state.isDarkMode = newValue;
      console.log("status ",state.isDarkMode)
    },
  },
});