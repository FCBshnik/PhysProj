<script lang="ts">
  import * as api from "$lib/services/ApiService";

  let works:api.WorkModel[] = [];

  async function refresh() {
    works = await api.service.listWorks();
  }

  refresh();

</script>

<article class="dark:bg-gray-800 dark:text-gray-400">
  <div class="p-4 flex flex-col h-screen">
    <div class="p-2 flex flex-row justify-between">
      <div>PhysLib</div>
      <div class="justify-items-stretch">
        <input class="rounded-md px-2 py-1" type="search"/>
        <button>Search</button>
      </div>
    </div>
    <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4">
      {#each works as work}
        <div class="border-2 rounded-md m-2 p-2 h-36 flex flex-col">
          <div class="grow flex justify-center items-center">
            <div class="text-center text-ellipsis overflow-hidden">
              {work.name}
            </div>
          </div>
          {#each work.authors as author}
          <div class="text-center text-ellipsis overflow-hidden text-sm italic">{author.name}</div>
          {/each}
        </div>
      {/each}
    </div>
  </div>
</article>
