<script lang="ts">
    import * as api from '$lib/services/ApiService';

    export let selected:api.WorkModel|undefined;

    let searchText:string = '';
    let list:api.WorkModel[] = [];
    let item:api.WorkModel|undefined;
    
    function search() {
        api.service.listWorks(searchText).then(r => {
            list = r;
            // workaround for issue: when <select> data source changed first selected <option> will not trigger on:change event
            // do it by hands
            if (list.length) {
                item = list[0];
                select();
            }
         });
    }

    function clear() {
        searchText = '';
        item = undefined;
        selected = undefined;
        list = [];
    }

    function select() {
        selected = item;
    }
</script>

<div class="flex flex-row gap-2 items-stretch">
    {#if list.length}
        <select bind:value={item} on:change={select}>
            {#each list as a}
            <option value={a}>{a.code}</option>
            {/each}
        </select>
        <button on:click={clear}>Clear</button>
    {:else}
        <input type="text" bind:value={searchText}/>
        <button on:click={search}>Search</button>
    {/if}
</div>