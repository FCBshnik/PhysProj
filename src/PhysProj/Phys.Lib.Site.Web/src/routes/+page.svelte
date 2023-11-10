<script lang="ts">
  import * as api from "$lib/services/ApiService";
  import { onMount } from "svelte";
  import DownloadFile from "$lib/components/DownloadFile.svelte";

  let works:api.WorkModel[] = [];
  let searchText:string = "";

  async function refresh() {
    works = await api.service.listWorks(searchText);
  }

  async function search() {
    refresh();
  }

  function getAuthorShort(author:api.AuthorModel){
    var parts = author.name?.split(' ');
    var initials = parts.slice(0, parts.length - 1).map(v => v[0] + '.');
    return parts[parts.length - 1] + ' ' + initials.join(' ');
  }

  onMount(() => refresh());

</script>

<article class="dark:bg-gray-800 dark:text-gray-400">
  <div class="p-4 flex flex-col h-full min-h-screen">
    <div class="p-2 flex flex-row items-center justify-between">
      <div><a href="/">PhysLib</a></div>
      <form on:submit={search}>
        <div class="flex flex-row">
          <input bind:value={searchText} class="rounded-l-md p-2 dark:bg-gray-400 dark:text-gray-800 dark:placeholder-gray-500" type="search" placeholder="Search"/>
          <button class="rounded-r p-2 dark:bg-gray-700 dark:hover:bg-gray-600" on:click={search}>
            <svg viewBox="0 0 20 20"
              fill="currentColor"
              class="h-6 w-10">
              <path d="M9 3.5a5.5 5.5 0 100 11 5.5 5.5 0 000-11zM2 9a7 7 0 1112.452 4.391l3.328 3.329a.75.75 0 11-1.06 1.06l-3.329-3.328A7 7 0 012 9z"/>
            </svg>
          </button>
        </div>
      </form>
      <div class=""></div>
    </div>
    <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
      {#each works as work}
        <div class="border-2 rounded-md m-2 p-2 h-36 flex flex-col dark:border-gray-500">
          <div class="grow flex justify-center items-center">
            <div class="text-center text-ellipsis overflow-hidden text-lg">
              {work.name}
            </div>
          </div>
          <div class="text-center text-ellipsis overflow-hidden italic">
            {#each work.authors || [] as author}
              <div class="inline px-1">{getAuthorShort(author)}</div>
            {/each}
          </div>
          <div class="text-end text-ellipsis overflow-hidden text-xs">
            {#each work.files || [] as file}
              <div class="inline px-1">
                <DownloadFile file={file}></DownloadFile>
              </div>
            {/each}
          </div>
        </div>
      {/each}
    </div>
  </div>
</article>
