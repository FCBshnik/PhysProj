<script lang="ts">
  import { createEventDispatcher } from 'svelte';

  export let searchText:string = "";

  const dispatch = createEventDispatcher();

  function search() {
    dispatch('onSearch')
  }
</script>

<form on:submit={search}>
  <div class="flex flex-row">
    <input bind:value={searchText} class="w-full rounded-l-md p-2 dark:bg-gray-400 dark:text-gray-800 dark:placeholder-gray-500" type="search" placeholder="Search"/>
    <button class="rounded-r p-2 dark:bg-gray-700 dark:hover:bg-gray-600" on:click={search}>
      <svg viewBox="0 0 20 20"
        fill="currentColor"
        class="h-6 w-10">
        <path d="M9 3.5a5.5 5.5 0 100 11 5.5 5.5 0 000-11zM2 9a7 7 0 1112.452 4.391l3.328 3.329a.75.75 0 11-1.06 1.06l-3.329-3.328A7 7 0 012 9z"/>
      </svg>
    </button>
  </div>
</form>