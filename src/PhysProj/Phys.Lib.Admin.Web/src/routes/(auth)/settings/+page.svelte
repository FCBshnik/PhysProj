<script lang="ts">
    import { JSONEditor } from 'svelte-jsoneditor'
    import * as api from '$lib/services/ApiService';
    
    let settings:string[] = [];
    let selectedSettings: string;
    let content = { json: {} };

    refresh();

    async function refresh() {
      settings = await api.service.listSettings();
    }

    async function getSettings() {
      content = { json: await api.service.getSettings(selectedSettings) };
    }

    async function saveSettings() {
      var settings = content.json || JSON.parse(content.text);
      await api.service.updateSettings(selectedSettings, settings);
    }

    function handleChange(updatedContent) {
      content = updatedContent
    }

</script>

<article class="flex flex-col grow gap-2">
  <section class="gap-2">Settings</section>
  <section class="flex flex-row gap-2">
    <div class="w-full">
      <select bind:value={selectedSettings} on:change={getSettings}>
        {#each settings as s}
          <option value={s}>{s}</option>
        {/each}
      </select>
		</div>
    <button class="w-1/12" on:click={refresh}>Refresh</button>
    <button class="w-1/12" on:click={saveSettings}>Save</button>
  </section>
  <section class="flex flex-col grow jse-theme-dark">
    <JSONEditor bind:content={content} onChange="{handleChange}" mode="text"/>
  </section>
</article>

<style>
  /* load one or multiple themes */
  @import 'svelte-jsoneditor/themes/jse-theme-dark.css';
</style>