<script lang="ts">
    import * as api from "$lib/services/ApiService";
    import { notificationsService } from "$lib/services/NotificationsService";

    export let file:api.FileModel;

    let downloadLink:api.FileLinkModel;

    function getDownloadLink() {
      api.service.getFileDownloadLink(file.code)
      .then(r => downloadLink = r)
      .catch(e => notificationsService.push(e));
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

{#if downloadLink}
  <a class="dark:text-cyan-600" rel="external" href="{downloadLink.url}" target="_blank">{file.format} {formatBytes(file.size)}</a>
{:else}
  <button on:click={getDownloadLink}>{file.format} {formatBytes(file.size)}</button>
{/if}