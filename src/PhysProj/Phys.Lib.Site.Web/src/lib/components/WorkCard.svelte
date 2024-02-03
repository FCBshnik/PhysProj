<script lang="ts">
  import type * as api from "$lib/services/ApiService";
  import DownloadFileButton from "$lib/components/DownloadFileButton.svelte";

  export let work:api.SearchResultWorkModel;
  export let result:api.SearchResultModel;

  function getAuthorShort(authorCode:string) {
    var author = result.authors?.find(a => a.code == authorCode);
    if (!author)
      return "";
    var parts = author.name?.split(' ') || [];
    var initials = parts.slice(0, parts.length - 1).map(v => v[0] + '.');
    return parts[parts.length - 1] + ' ' + initials.join(' ');
  }
</script>

<div class="border-2 rounded-md m-2 p-2 flex flex-col dark:border-gray-500 border-gray-400">
  <div class="grow flex justify-center items-center">
    <div class="text-center text-ellipsis overflow-hidden text-lg">
      {work.name}
    </div>
  </div>
  <div class="text-center text-ellipsis overflow-hidden italic">
    {#each work.authors || [] as author}
      <div class="inline-block p-1">{getAuthorShort(author)}</div>
    {/each}
  </div>
  <div class="text-end text-ellipsis overflow-hidden text-xs">
    {#each work.files || [] as file}
      <div class="p-1">
        <DownloadFileButton file={file}/>
      </div>
    {/each}
  </div>
</div>