<template>
    <div class="app">
        <router-view></router-view>
    </div>
</template>

<script>
    export default {
         name: 'App'}
</script>
<style lang="scss">

#app {
    min-height: 100vh;
}

* {  
    margin: 0;
    padding: 0;
    box-sizing: border-box;
    background-size: cover; 
    background-repeat: no-repeat;
}


</style>