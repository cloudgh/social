<template>
  <div class="app3" :class="{ 'dark-mode': isDarkMode }">
    <h1>Photos</h1>
    <div class="photo-gallery">
      <div v-if="loadingError" class="error-message">
        Произошла ошибка при загрузке данных. Пожалуйста, проверьте ваше соединение и попробуйте позже.
      </div>
      <div v-for="(photo, index) in photos" :key="index" class="photo-item" @click="photo.isLoaded && bigPhoto(photo.url)">
        <span v-if="photo.authorId == this.my_id || this.isadmin==='true'" class="delPhoto" @click.stop="del_photo(photo.id)"  >
          <span>&#128465;</span>
        </span>
        <img v-show="photo.isLoaded" :src="photo.url" :alt="photo.description" />
        <div v-show="!photo.isLoaded" class="photo-placeholder"></div>
      </div>
    </div>


    <div v-if="isModalOpen" class="modal-overlay">
      <div class="modal-content">
        <span class="close-btn" @click="closeModal">×</span>
        <img :src="modalImageUrl" alt="Large Photo" />
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  data() {
    return {
      my_id:null,
      isadmin:null,
    };
  },
  props: {
    photos: {
      type: Array,
      default: () => [],
      required: true
    }
  },
  model: {
    prop: 'photos',
    event: 'update:photos'
  },
  data() {
    return {
      isModalOpen: false,
      modalImageUrl: "",
      loadingError: false,
    };
  },
  async mounted() {

    this.isadmin = localStorage.getItem('isAdmin');
    this.my_id=localStorage.getItem('userId');
    try {
      const username = this.$route.params.username;
      const response = await axios.get(`https://localhost:7297/api/users/${username}`);
      this.$emit('update:photos', response.data.photos.map(photo => ({
        ...photo,
        url: `https://localhost:7297${photo.url}`,
        isLoaded: true
      })));
    } catch (error) {
      this.loadingError = true;
    }
  },
  computed: {
    isDarkMode() {
      return this.$store.state.isDarkMode;
    },
  },
  methods: {
    bigPhoto(photoUrl) {
      this.modalImageUrl = photoUrl;
      this.isModalOpen = true;
    },
    closeModal() {
      this.isModalOpen = false;
      this.modalImageUrl = "";
    },
    async del_photo(photoId) {
      try {
        await axios.delete(`https://localhost:7297/api/photos/${photoId}`, {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('token')}`
          }
        });
        const updatedPhotos = this.photos.filter(photo => photo.id !== photoId);
        this.$emit('update:photos', updatedPhotos); 
      } catch (error) {
        console.error('Error deleting photo:', error);
        
      }
    },
  },
};
</script>

<style scoped>
.error-message {
  padding: 10px;
  background-color: #faccff;
  border: 1px solid #ae00ff;
  margin: 10px 0;
  color: #333;
  text-align: center;
}
.photo-placeholder {
  width: 100%;
  height: 150px;
  background-color: #ccc; 
  border-radius: 20px;
  animation: loadingAnimation 2s infinite alternate;
}

@keyframes loadingAnimation {
  0% {
    background-color: #ccc;
  }
  50% {
    background-color: #dfdfdf;
  }
  100% {
    background-color: #ccc;
  }
}
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  align-items: center;
  justify-content: center;
  animation: bendText 2s 1;
  z-index: 3;
  
}
@keyframes bendText{
  0% {
    transform: scale(0.1);
  }
  100% {
    transform:  scale(1);
  }
}
.modal-content {

  max-width: 90vw;
  max-height: 90vh;
  position: fixed;
  
}

.modal-content img {
  max-width: 90vw;
  max-height: 90vh;
  object-fit: cover;
}
.delPhoto{
  position: relative;
  top:35px;
  z-index: 3;
  display: flex;
  justify-content: end;
  span{
    font-size: 25px;
    border-radius:20px; 
    background-color: antiquewhite;
    cursor: pointer;
    z-index: 3;
  }
}
.close-btn {
  position: absolute;
  top: 0;
  right: 0;
  font-size: 20px;
  cursor: pointer;
  color: #ffffff;
}
h1{
    display: flex;
    justify-content: center;
    text-align: center;
    margin-bottom: 5px;
  }
.photo-gallery {
  display: flex;
  flex-direction: row;
  flex-wrap: wrap;
  gap: 20px;
  justify-content: center;
 
}

.photo-item {
  flex-basis: 20vw;
  transition: all 0.8s cubic-bezier(0.25, 0.4, 0.45, 1.4);
}

.photo-item img {
  width: 100%;
  height: 200px;
  border-radius: 20px;
  object-fit: cover;
}

.dark-mode {
    background-color: rgba(23, 23, 23);
    color: white;
    min-height: 100vh;
  }
</style>
