<script lang="ts">
	import * as api from '$lib/services/ApiService';
	import { onMount } from 'svelte';
	import WorkCard from '$lib/components/WorkCard.svelte';
	import SearchInput from '$lib/components/SearchInput.svelte';
  import ThemeToggle from '$lib/components/ThemeToggle.svelte';

	let works: api.WorkModel[] = [];
	let searchText: string = '';
	let loading: boolean = false;
  
	function refresh() {
		works = [];
		loading = true;
		api.service
			.listWorks(searchText)
			.then((r) => (works = r))
			.finally(() => (loading = false));
	}

	onMount(() => refresh());
</script>

<article class="dark:bg-gray-800 dark:text-gray-400 bg-gray-300 text-black">
	<div class="p-4 flex flex-col h-full min-h-screen">
		<div class="p-2 flex flex-row items-center justify-between">
			<div><a href="/">PhysLib</a></div>
			<div class="w-2/3 flex flex-row">
				<div class="w-full">
					<SearchInput bind:searchText on:onSearch={refresh} />
				</div>
			</div>
			<div class="flex flex-col justify-center">
        <ThemeToggle/>
      </div>
		</div>
		<div class="">
			<div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
				{#each works as work}
					<WorkCard {work} />
				{/each}
			</div>
			{#if !loading && !works.length}
				<div class="text-center">Nothing found</div>
			{/if}
			{#if loading}
				<div
					class="absolute m-auto left-0 right-0 top-0 bottom-0 animate-spin w-10 h-10 border-[2px] border-current border-t-transparent rounded-full dark:text-gray-300"
					role="status"
					aria-label="loading"
				/>
			{/if}
		</div>
	</div>
</article>
