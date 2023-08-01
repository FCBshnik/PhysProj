<script lang="ts">
  import * as api from '$lib/services/ApiService';

  export let selected:api.FileModel|undefined;

  let searchText:string = '';
  let files:api.FileModel[] = [];
  let file:api.FileModel|undefined;
  
  function search() {
      api.service.listFiles(searchText).then(r => {
          files = r;
          // workaround for issue: when <select> data source changed first selected <option> will not trigger on:change event
          // do it by hands
          if (files.length) {
              file = files[0];
              select();
          }
       });
  }

  function clear() {
      searchText = '';
      file = undefined;
      selected = undefined;
      files = [];
  }

  function select() {
      selected = file;
  }
</script>

<div class="flex flex-row gap-2 items-stretch">
  {#if files.length}
      <select bind:value={file} on:change={select}>
          {#each files as a}
          <option value={a}>{a.code}</option>
          {/each}
      </select>
      <button on:click={clear}>Clear</button>
  {:else}
      <input type="text" bind:value={searchText}/>
      <button on:click={search}>Search</button>
  {/if}
</div>