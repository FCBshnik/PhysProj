<script lang="ts">
    import * as api from '$lib/services/ApiService';

    let authors:api.AuthorModel[] = [];
    let text:string = '';

    async function refresh() {
        authors = await api.service.listAuthors(text);
    }

    async function create() {
        api.service.createAuthor(new api.AuthorCreateModel({ code: text })).then(r => {
            text = '';
            refresh()
        })
    }

    async function deleteAuthor(author:api.AuthorModel) {
        api.service.deleteAuthor(author.code).then(r => {
            refresh()
        });
    }

    refresh();
</script>

<article class="flex flex-col grow gap-2">
    <section class="gap-2">Authors</section>
    <section class="flex flex-row items-center gap-2">
        <input class="w-full" type="search" bind:value={text} placeholder="Code to create new author or text to search"/>
        <button class="w-auto" on:click={refresh}>Search</button>
        <button class="w-auto" on:click={create}>Create</button>
        <button class="w-auto" on:click={refresh}>Refresh</button>
    </section>

    <section class="flex flex-row items-center gap-2">
        <table class="table-auto w-full">
            <thead class="">
              <tr class="border-b-2 border-b-gray-700">
                <th>Code</th>
                <th>Born</th>
                <th>Died</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
                {#each authors as author}
                <tr class="border-b-2 border-b-gray-700">
                    <td><a href="/authors/{author.code}">{author.code}</a></td>
                    <td>{author.born ?? '-'}</td>
                    <td>{author.died ?? '-'}</td>
                    <td class="flex justify-end">
                        <button class="w-min text-xs" on:click={() => deleteAuthor(author)}>X</button>
                    </td>
                  </tr>
                {/each}
            </tbody>
          </table>
    </section>
</article>