<script lang="ts">
    import * as api from '$lib/services/ApiService';

    let works:api.WorkModel[] = [];
    let text:string = '';

    async function refresh() {
        works = await api.service.listWorks(text);
    }

    async function create() {
        api.service.createWork(new api.WorkCreateModel({ code: text })).then(r => {
            text = '';
            refresh()
        })
    }

    async function deleteWork(work:api.WorkModel) {
        api.service.deleteWork(work.code).then(r => {
            refresh()
        });
    }

    function updateIsPublic(work:api.WorkModel, isPublic: boolean) {
      api.service
        .updateWork(
          work.code,
          new api.WorkUpdateModel({ isPublic: isPublic })
        )
        .finally(refresh);
    }

    refresh();
</script>

<article class="flex flex-col grow gap-2">
    <section class="gap-2">Works</section>
    <section class="">
      <div class="flex flex-row gap-2 justify-end">
          <button class="w-1/6 text-red-300" on:click={() => api.service.invalidateWorksCache()}>Invalidate cache</button>
      </div>
  </section>
    <section class="">
        <!-- svelte-ignore a11y-autofocus -->
        <form class="max-w-full bg-inherit p-0" autofocus>
          <div class="flex flex-row gap-2 items-center">
            <input class="w-full" type="search" bind:value={text} placeholder="Code to create new work or text to search"/>
            <button class="w-1/12" on:click={refresh}>Search</button>
            <button class="w-1/12" on:click={create}>Create</button>
            <button class="w-1/12" on:click={refresh}>Refresh</button>
          </div>
        </form>
    </section>

    <section class="flex flex-row items-center gap-2">
        <table class="table-auto w-full">
            <thead class="">
              <tr class="border-b-2 border-b-gray-700">
                <th>Code</th>
                <th>Date</th>
                <th>Lang</th>
                <th>Authors</th>
                <th>Files</th>
                <th>Public</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
                {#each works as work}
                <tr class="border-b-2 border-b-gray-700">
                    <td><a href="/works/{work.code}">{work.code}</a></td>
                    <td>{work.publish ?? ''}</td>
                    <td>{work.language ?? ''}</td>
                    <td class="flex flex-col">
                      {#each work.authorsCodes ?? [] as authorCode}
                        <a href="/authors/{authorCode}">{authorCode}</a>
                      {/each}
                    </td>
                    <td>
                      {work.filesCodes?.join(', ')}
                    </td>
                    <td>
                      <button class="w-min text-xs" on:click={() => updateIsPublic(work, work.isPublic !== true)}>{work.isPublic}</button>
                    </td>
                    <td class="flex justify-end">
                        <button class="w-min text-xs" on:click={() => deleteWork(work)}>X</button>
                    </td>
                  </tr>
                {/each}
            </tbody>
          </table>
    </section>
</article>