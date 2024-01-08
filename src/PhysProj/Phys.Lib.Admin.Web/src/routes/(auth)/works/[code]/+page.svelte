<script lang="ts">
	import { page } from '$app/stores';
	import * as api from '$lib/services/ApiService';
	import { goto } from '$app/navigation';
	import LanguageSelector from '$lib/components/LanguageSelector.svelte';
	import AuthorSelector from '$lib/components/AuthorSelector.svelte';
	import WorkSelector from '$lib/components/WorkSelector.svelte';
	import FileSelector from '$lib/components/FileSelector.svelte';

	let work: api.WorkModel;
	let selectedLanguage: string = 'en';

	let author: api.AuthorModel | undefined;
	let subWork: api.WorkModel | undefined;
	let originalWork: api.WorkModel | undefined;
	let file: api.FileModel | undefined;

	refresh();

	function refresh() {
		api.service
			.getWork($page.params.code)
			.then((a) => (work = a))
			.catch((e) => goto('/404'));
	}

	function updateDate() {
		api.service
			.updateWork(
				work.code,
				new api.WorkUpdateModel({ date: work.publish })
			)
			.finally(refresh);
	}

	function updateLanguage() {
		api.service
			.updateWork(
				work.code,
				new api.WorkUpdateModel({ language: work.language })
			)
			.finally(refresh);
	}

  function updateIsPublic(isPublic: boolean) {
    api.service
      .updateWork(
        work.code,
        new api.WorkUpdateModel({ isPublic: isPublic })
      )
      .finally(refresh);
  }

	function updateInfo(info: api.WorkInfoModel) {
		api.service
			.updateWorkInfo(
				work.code,
				info.language,
				new api.WorkInfoUpdateModel({ name: info.name, description: info.description })
			)
			.finally(refresh);
	}

	function addInfo() {
		work.infos?.push(new api.WorkInfoModel({ language: selectedLanguage }));
		work.infos = work.infos;
	}

	function deleteInfo(info: api.WorkInfoModel) {
		api.service.deleteWorkInfo(work.code, info.language).finally(refresh);
	}

	function searchAuthor() {
		api.service.listAuthors(authorSearchText).then(a => author = a.find(_ => true));
	}

	function linkAuthor() {
		console.debug('linkAuthor');
		api.service.linkAuthorToWork(work.code, author?.code).finally(refresh);
	}	

	function unlinkAuthor(authorCode:string) {
		api.service.unlinkAuthorFromWork(work.code, authorCode).finally(refresh);
	}

	function linkSubWork() {
		api.service.linkWorkToCollectedWork(work.code, subWork?.code).finally(refresh);
	}	

	function unlinkSubWork(subWorkCode:string) {
		api.service.unlinkWorkFromCollectedWork(work.code, subWorkCode).finally(refresh);
	}

	function searchOriginalWork() {
		api.service.listWorks(originalSearchText).then(a => originalWork = a.find(_ => true));
	}

	function linkOriginalWork() {
		api.service.linkOriginalToWork(work.code, originalWork?.code).finally(refresh);
	}	

	function unlinkOriginalWork() {
		api.service.unlinkOriginalFromWork(work.code).finally(refresh);
	}

  function linkFile() {
    api.service.linkFileToWork(work.code, file?.code).finally(refresh);
  }	

  function unlinkFile(fileCode:string) {
    api.service.unlinkFileFromWork(work.code, fileCode).finally(refresh);
  }
</script>

<article class="p-4 gap-1">
	{#if work}
		<section class="p-2"><a href="/works">Works</a> / '{work.code}'</section>
		<section class="grid grid-flow-col justify-stretch p-2 border-b-2 border-b-gray-700">
			<div>
				<div class="p-2">Date</div>
				<div class="flex flex-row gap-2 p-2">
					<input class="basis-10/12" type="text" bind:value={work.publish} />
					<button class="basis-2/12" on:click={updateDate}>Update</button>
				</div>
			</div>
			<div>
				<div class="p-2">Language</div>
				<div class="flex flex-row gap-2 p-2">
					<div class="basis-10/12"><LanguageSelector bind:language={work.language}/></div>
					<button class="basis-2/12" on:click={updateLanguage}>Update</button>
				</div>
			</div>
			<div>
				<div class="p-2">Is Public</div>
				<div class="flex flex-row gap-2 p-2">
					<button on:click={() => updateIsPublic(work.isPublic !== true)}>
            {#if work.isPublic}
              Hide
            {:else}
              Show
            {/if}
          </button>
				</div>
			</div>
		</section>
		<section class="p-2 border-b-2 border-b-gray-700">
			<div class="flex flex-row gap-2 p-2 items-center justify-between">
				<div class="basis-1/12">Info</div>
				<div class="basis-3/12">Name</div>
                <div class="basis-6/12">Short description</div>
				<div class="basis-2/12 grid grid-flow-col justify-stretch gap-2">
					<LanguageSelector bind:language={selectedLanguage}/>
					<button class="" on:click={addInfo}>Add</button>
				</div>
			</div>
			{#each work.infos ?? [] as info}
            <div class="flex flex-row gap-2 p-2 items-center">
                <div class="basis-1/12 text-center">{info.language}</div>
                <input class="basis-3/12" type="text" bind:value={info.name} />
                <input class="basis-6/12" type="text" bind:value={info.description} />
                <div class="basis-2/12 flex flex-row gap-2">
                    <button on:click={() => updateInfo(info)}>Update</button>
                    <button on:click={() => deleteInfo(info)}>X</button>
                </div>
            </div>
			{/each}
		</section>
		<section class="p-2 border-b-2 border-b-gray-700">
			<div class="p-2">Authors</div>
			<div class="flex flex-row gap-2 p-2 items-center">
				<div class="basis-6/12"><AuthorSelector bind:selected={author}/></div>
				<div class="basis-4/12">{author?.code ?? ''}</div>
				<button class="basis-2/12 disabled:opacity-75" on:click={linkAuthor} disabled='{author === undefined}'>Link</button>
			</div>
			{#each work.authorsCodes ?? [] as authorCode}
            <div class="flex flex-row gap-2 p-2">
                <div class="basis-10/12">{authorCode}</div>
                <div class="basis-2/12 flex flex-row gap-2">
                    <button on:click={() => unlinkAuthor(authorCode)}>Unlink</button>
                </div>
            </div>
			{/each}
		</section>
		<section class="p-2 border-b-2 border-b-gray-700">
			<div class="p-2">Original</div>
			<div class="flex flex-row gap-2 p-2 items-center">
				{#if work.originalCode}
					<div class="basis-10/12">{work.originalCode}</div>
					<button class="basis-2/12 disabled:opacity-75" on:click={unlinkOriginalWork}>Unlink</button>
				{:else}
					<div class="basis-6/12"><WorkSelector bind:selected={originalWork}/></div>
					<div class="basis-4/12">{originalWork?.code ?? ''}</div>
					<button class="basis-2/12 disabled:opacity-75" on:click={linkOriginalWork} disabled='{originalWork === undefined}'>Link</button>
				{/if}
			</div>
		</section>
		<section class="p-2 border-b-2 border-b-gray-700">
			<div class="p-2">Sub works</div>
			<div class="flex flex-row gap-2 p-2 items-center">
				<div class="basis-6/12"><WorkSelector bind:selected={subWork}/></div>
				<div class="basis-4/12">{subWork?.code ?? ''}</div>
				<button class="basis-2/12 disabled:opacity-75" on:click={linkSubWork} disabled='{subWork === undefined}'>Link</button>
			</div>
			{#each work.subWorksCodes ?? [] as subWorkCode}
            <div class="flex flex-row gap-2 p-2">
                <div class="basis-10/12">{subWorkCode}</div>
                <div class="basis-2/12 flex flex-row gap-2">
                    <button on:click={() => unlinkSubWork(subWorkCode)}>Unlink</button>
                </div>
            </div>
			{/each}
		</section>
		<section class="p-2 border-b-2 border-b-gray-700">
			<div class="p-2">Files</div>
			<div class="flex flex-row gap-2 p-2 items-center">
				<div class="basis-6/12"><FileSelector bind:selected={file}/></div>
				<div class="basis-4/12">{file?.code ?? ''}</div>
				<button class="basis-2/12 disabled:opacity-75" on:click={linkFile} disabled='{file === undefined}'>Link</button>
			</div>
			{#each work.filesCodes ?? [] as fileCode}
            <div class="flex flex-row gap-2 p-2">
                <div class="basis-10/12">{fileCode}</div>
                <div class="basis-2/12 flex flex-row gap-2">
                    <button on:click={() => unlinkFile(fileCode)}>Unlink</button>
                </div>
            </div>
			{/each}
		</section>
	{/if}
</article>
