<script lang="ts">
    import * as api from '$lib/services/ApiService';

    let authors:api.AuthorModel[] = [];
    let text:string = '';

    async function refresh() {
        authors = await api.service.listAuthors();
    }

    async function create() {
        api.service.createAuthor(new api.AuthorCreateModel({ code: text })).then(r => {
            text = '';
            refresh()
        })
    }

    refresh();
</script>

<article>
    <section class="flex flex-row items-center gap-4">
        <input class="w-full" type="search" bind:value={text} placeholder="Code to create new author or text to search"/>
        <button class="w-auto" on:click={refresh}>Search</button>
        <button class="w-auto" on:click={create}>Create</button>
        <button class="w-auto" on:click={refresh}>Refresh</button>
    </section>

    <section class="p-4">
        <table class="table-auto w-full">
            <thead>
              <tr>
                <th>Code</th>
                <th>Born</th>
                <th>Died</th>
                <th>Edit</th>
              </tr>
            </thead>
            <tbody>
                {#each authors as author}
                <tr class="border-b-2 border-b-gray-700">
                    <td><a href="/authors/{author.code}">{author.code}</a></td>
                    <td>{author.born ?? '-'}</td>
                    <td>{author.died ?? '-'}</td>
                    <td class="flex justify-end gap-1">
                        <button class="w-min text-xs" on:click={refresh}>X</button>
                    </td>
                  </tr>
                {/each}
            </tbody>
          </table>
    </section>
</article>