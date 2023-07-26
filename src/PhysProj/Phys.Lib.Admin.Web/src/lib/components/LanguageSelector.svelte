<script context="module" lang="ts">
    import * as api from '$lib/services/ApiService';

	let languages:api.LanguageModel[] = [];

    api.service.listLanguages().then(r => languages = r);
</script>

<script lang="ts">
    export let language:string|undefined;
</script>

<div>
    <select bind:value={language}>
        {#each languages as lang}
        <option value={lang.code}>{lang.name}</option>
        {/each}
    </select>
</div>