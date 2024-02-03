<script lang="ts">
    import * as api from "$lib/services/ApiService";
    import { notificationsService } from "$lib/services/NotificationsService";

    export let file:api.SearchResultFileModel

    let downloadLink:api.FileLinkModel
    let loading:boolean = false

    function getDownloadLink() {
      loading = true;
      api.service.getFileDownloadLink(file.code)
        .then(r => downloadLink = r)
        .catch(e => notificationsService.push(e))
        .finally(() => loading = false)
    }

    function formatBytes(bytes:number, decimals = 0) {
      if (!+bytes) 
        return '0 Bytes'

      const k = 1000
      const dm = decimals < 0 ? 0 : decimals
      const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB']
      const i = Math.floor(Math.log(bytes) / Math.log(k))
      return `${parseFloat((bytes / Math.pow(k, i)).toFixed(dm))} ${sizes[i]}`
  }
</script>

<div class="">
  {#if downloadLink}
    <a class="underline italic text-gray-100" rel="external" href="{downloadLink.url}" target="_blank">{file.format} {formatBytes(file.size)}</a>
  {:else}
    <button on:click={getDownloadLink} class="underline italic rounded-md disabled:text-gray-600" disabled={loading}>
      {#if loading}
        <div class="absolute translate-x-3/4">
          <div class="animate-spin w-5 h-5 border-[2px] border-current border-t-transparent rounded-full dark:text-gray-300" role="status" aria-label="loading">
          </div>
        </div>
      {/if}
      {file.format} {formatBytes(file.size)}
    </button>
{/if}
</div>