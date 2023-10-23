<script lang="ts">
	import * as api from '$lib/services/ApiService';

	let files: api.FileModel[] = [];
	let filesSearch: string = '';

	let storages: api.FileStorageModel[] = [];
  let selectedStorage: api.FileStorageModel | undefined;
  let storageFilesSearch: string = '';
  let storageFiles: api.FileStorageFileModel[] = [];
  let selectedStorageFile: api.FileStorageFileModel | undefined;

	api.service.listStorages().then((r) => {
    storages = r;
    selectedStorage = storages[0];
  });

	async function refreshStorage() {
		await api.service.refreshStorage(selectedStorage.code);
	}

	async function refreshFiles() {
		files = await api.service.listFiles(filesSearch);
	}

  async function refreshStorageFiles() {
    storageFiles = await api.service.listStorageFiles(selectedStorage.code, storageFilesSearch);
    if (storageFiles.length)
      selectedStorageFile = storageFiles[0];
	}

  async function linkStorageFile() {
    await api.service.linkStorageFile(selectedStorage.code, new api.FileStorageLinkModel({ fileId: selectedStorageFile?.fileId }));
    refreshFiles();
  }

	async function deleteFile(file: api.FileModel) {
    await api.service.deleteFile(file.code).then((_) => refreshFiles());
	}

	refreshFiles();
</script>

<article>
  <section class="p-2">Storage files</section>
	<section class="flex flex-row items-center p-4 gap-4">
		<div class="w-1/6">
      <select bind:value={selectedStorage}>
        {#each storages as s}
          <option value={s}>{s.name}</option>
        {/each}
      </select>
		</div>
    <input class="w-1/12" type="search" bind:value={storageFilesSearch} placeholder="Text to search file" />
    <button class="w-auto" on:click={refreshStorageFiles}>Search</button>
    <div class="w-full">
      <select bind:value={selectedStorageFile}>
        {#each storageFiles as f}
          <option value={f}>{f.name}</option>
        {/each}
      </select>
		</div>
    <button class="w-auto" on:click={linkStorageFile} disabled={selectedStorageFile == undefined}>Link</button>
	</section>
  <section class="flex flex-row items-center p-4 gap-4">
    <button class="w-auto" on:click={refreshStorage}>Refresh</button>
  </section>

  <section class="p-2">Files</section>
	<section class="flex flex-row items-center p-4 gap-4">
		<input class="w-full" type="search" bind:value={filesSearch} placeholder="Text to search file" />
		<button class="w-auto" on:click={refreshFiles}>Search</button>
		<button class="w-auto" on:click={refreshFiles}>Refresh</button>
	</section>

	<section class="p-4">
		<table class="table-auto w-full">
			<thead class="">
				<tr class="border-b-2 border-b-gray-700">
					<th>Code</th>
					<th>Format</th>
					<th>Size</th>
          <th>Links</th>
					<th />
				</tr>
			</thead>
			<tbody>
				{#each files as file}
					<tr class="border-b-2 border-b-gray-700">
						<td><a href="/works/{file.code}">{file.code}</a></td>
						<td>{file.format ?? '-'}</td>
						<td>{file.size ?? '-'}</td>
            <td>
              {#each file.links as link}
                [{link.storageCode}] {link.fileId}
              {/each}
            </td>
						<td class="flex justify-end">
							<button class="w-min text-xs" on:click={() => deleteFile(file)}>X</button>
						</td>
					</tr>
				{/each}
			</tbody>
		</table>
	</section>
</article>
