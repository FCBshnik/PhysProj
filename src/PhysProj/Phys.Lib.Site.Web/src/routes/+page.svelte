<script lang="ts">
  import * as api from "$lib/services/ApiService";
  import { onMount } from "svelte";
  import WorkCard from "$lib/components/WorkCard.svelte";
  import SearchInput from "$lib/components/SearchInput.svelte";

  let works:api.WorkModel[] = [];
  let searchText:string = "";

  async function refresh() {
    works = await api.service.listWorks(searchText);
  }

  onMount(() => refresh());

</script>

<article class="dark:bg-gray-800 dark:text-gray-400">
  <div class="p-4 flex flex-col h-full min-h-screen">
    <div class="p-2 flex flex-row items-center justify-between">
      <div><a href="/">PhysLib</a></div>
      <div class="w-2/3 flex flex-row">
        <div class="w-full">
          <SearchInput bind:searchText={searchText} on:onSearch={refresh}/>
        </div>
      </div>
      <div class="">Settings</div>
    </div>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      {#each works as work}
        <WorkCard work={work}/>
      {/each}
    </div>
  </div>
</article>
