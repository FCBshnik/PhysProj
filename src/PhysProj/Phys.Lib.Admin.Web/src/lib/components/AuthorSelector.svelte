<script lang="ts">
    import * as api from '$lib/services/ApiService';

    export let selected:api.AuthorModel|undefined;

    let searchText:string = '';
    let authors:api.AuthorModel[] = [];
    let author:api.AuthorModel|undefined;
    
    function search() {
        api.service.listAuthors(searchText).then(r => {
            authors = r;
            // workaround for issue: when <select> data source changed first selected <option> will not trigger on:change event
            // do it by hands
            if (authors.length) {
                author = authors[0];
                select();
            }
         });
    }

    function clear() {
        searchText = '';
        author = undefined;
        selected = undefined;
        authors = [];
    }

    function select() {
        selected = author;
    }
</script>

<div class="flex flex-row gap-2 items-stretch">
    {#if authors.length}
        <select bind:value={author} on:change={select}>
            {#each authors as a}
            <option value={a}>{a.code}</option>
            {/each}
        </select>
        <button on:click={clear}>Clear</button>
    {:else}
        <input type="text" bind:value={searchText}/>
        <button on:click={search}>Search</button>
    {/if}
</div>