<script lang="ts">
  import type * as api from "$lib/services/ApiService";
  import InformationIcon from "$lib/components/icons/Information.svelte";
  import DownloadFileButton from "$lib/components/DownloadFileButton.svelte";

  let showInfo = false;

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

<div class="relative border-2 rounded-md m-2 p-2 flex flex-col dark:border-gray-500 border-gray-400">
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
  <div class="flex flex-row justify-between">
    <div class="text-end text-ellipsis overflow-hidden text-xs">
      {#if work.subWorks?.length}
      <button type="button" class="" on:click={() => showInfo = !showInfo}>
        <InformationIcon></InformationIcon>
      </button>
      {/if}
    </div>
    <div class="text-end text-ellipsis overflow-hidden text-xs">
      {#each work.files || [] as file}
        <div class="p-1">
          <DownloadFileButton file={file}/>
        </div>
      {/each}
    </div>
  </div>

  {#if showInfo}
    <!-- svelte-ignore a11y-click-events-have-key-events -->
    <!-- svelte-ignore a11y-no-noninteractive-element-interactions -->
    <div role="tooltip" class="absolute top-0 left-0 w-full min-h-full z-50 border-2 rounded-md text-sm bg-gray-200 border-gray-400 dark:border-gray-600 dark:bg-gray-900" on:click={() => showInfo = false}>
      <div class="p-2 rounded-t-md text-center dark:bg-gray-700" >
          {work.name}
      </div>
      <div class="p-2 rounded-md">
        {#each work.subWorks || [] as subWork}
          <div>
            <span class="italic">{subWork.authors?.map(getAuthorShort).join(', ')}</span> - <span>{subWork.name}</span>
            <span class="italic">{subWork.isTranslation ? ' (Перевод)' : ''}</span>
          </div>
        {/each}
      </div>
    </div>
  {/if}
</div>