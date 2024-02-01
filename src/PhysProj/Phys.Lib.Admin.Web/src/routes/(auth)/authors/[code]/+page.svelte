<script lang="ts">
	import { page } from '$app/stores';
	import * as api from '$lib/services/ApiService';
	import { goto } from '$app/navigation';

	let author: api.AuthorModel;
	let selectedLanguage: string = 'en';

	refresh();

	function refresh() {
		api.service
			.getAuthor($page.params.code)
			.then((a) => (author = a))
			.catch((e) => goto('/404'));
	}

	function updateLifetime() {
		api.service
			.updateAuthorLifetime(
				author.code,
				new api.AuthorLifetimeUpdateModel({ born: author?.born?.trim(), died: author?.died.trim() })
			)
			.finally(refresh);
	}

	function updateInfo(info: api.AuthorInfoModel) {
		api.service
			.updateAuthorInfo(
				author.code,
				info.language,
				new api.AuthorInfoUpdateModel({ fullName: info.fullName?.trim(), description: info.description?.trim() })
			)
			.finally(refresh);
	}

	function addInfo() {
		author.infos?.push(new api.AuthorInfoModel({ language: selectedLanguage }));
		author.infos = author.infos;
	}

	function deleteInfo(info: api.AuthorInfoModel) {
		api.service.deleteAuthorInfo(author.code, info.language).finally(refresh);
	}
</script>

<article class="p-4 gap-1">
	{#if author}
		<section class="p-2"><a href="/authors">Authors</a> / '{author.code}'</section>
		<section class="p-2 border-b-2 border-b-gray-700">
			<div class="p-2">Lifetime</div>
			<div class="flex flex-row gap-2 p-2">
				<input class="basis-5/12" type="text" bind:value={author.born} />
				<input class="basis-5/12" type="text" bind:value={author.died} />
				<button class="basis-2/12" on:click={updateLifetime}>Update</button>
			</div>
		</section>
		<section class="p-2 border-b-2 border-b-gray-700">
			<div class="flex flex-row gap-2 p-2 items-center justify-between">
				<div class="basis-1/12">Info</div>
				<div class="basis-3/12">Full name</div>
                <div class="basis-6/12">Short description</div>
				<div class="basis-2/12 flex flex-row gap-2">
					<select bind:value={selectedLanguage} class="">
						<option>en</option>
						<option>ru</option>
					</select>
					<button class="" on:click={addInfo}>Add</button>
				</div>
			</div>
			{#each author.infos ?? [] as info}
            <div class="flex flex-row gap-2 p-2 items-center">
                <div class="basis-1/12 text-center">{info.language}</div>
                <input class="basis-3/12" type="text" bind:value={info.fullName} />
                <input class="basis-6/12" type="text" bind:value={info.description} />
                <div class="basis-2/12 flex flex-row gap-2">
                    <button on:click={() => updateInfo(info)}>Update</button>
                    <button on:click={() => deleteInfo(info)}>X</button>
                </div>
            </div>
			{/each}
		</section>
	{/if}
</article>
