<script lang="ts">
    import * as api from '$lib/services/ApiService';

    let authors:api.AuthorModel[] = [];
    let text:string = '';

    async function refresh() {
        authors = await api.service.listAuthors();
    }

    async function create() {
        api.service.createAuthor(new api.AuthorCreateModel({ code: text })).then(r => refresh())
    }

    refresh();
</script>

<article>
    <section class="flex flex-row items-center gap-4">
        <input class="w-full" type="search" bind:value={text}/>
        <button class="w-auto" on:click={refresh}>Search</button>
        <button class="w-auto" on:click={create}>Create</button>
        <button class="w-auto" on:click={refresh}>Refresh</button>
    </section>

    <section class="p-4">
        {#each authors as author}
            <div>{author.code}</div>
        {/each}
    </section>
</article>