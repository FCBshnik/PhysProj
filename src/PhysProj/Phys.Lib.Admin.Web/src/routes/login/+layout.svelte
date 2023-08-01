<script lang="ts">
  import Footer from "$lib/components/Footer.svelte";
</script>

<div class="flex flex-col h-screen">
  <div class="flex-grow flex justify-center items-center">
    <slot />
  </div>
  <div>
    <Footer/>
  </div>
</div>
