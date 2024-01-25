<script lang="ts">
    import { JSONEditor, Mode } from 'svelte-jsoneditor'
    import * as api from '$lib/services/ApiService';

    let libStats:api.SystemStatsModel | undefined;
    let content = { json: {} };

    async function refresh() {
        libStats = await api.service.getLibraryStats();
        content = { json: libStats };
    }

    refresh();
</script>

{#if libStats}
  <section class="flex flex-col grow jse-theme-dark">
    <JSONEditor bind:content={content} mode={Mode.text} readOnly mainMenuBar={false} navigationBar={false}/>
  </section>
{/if}

<style>
  /* load one or multiple themes */
  @import 'svelte-jsoneditor/themes/jse-theme-dark.css';
</style>