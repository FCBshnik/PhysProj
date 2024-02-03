<script lang="ts">
	import * as api from '$lib/services/ApiService';
  import logo from '$lib/assets/logo.png';
	import { onMount } from 'svelte';
	import WorkCard from '$lib/components/WorkCard.svelte';
	import SearchInput from '$lib/components/SearchInput.svelte';
  import ThemeToggle from '$lib/components/ThemeToggle.svelte';

	let result: api.SearchResultModel | undefined;
	let searchText: string = '';
	let loading: boolean = false;
  
	function refresh() {
		result = undefined;
		loading = true;
		api.service
			.search(searchText)
			.then((r) => (result = r))
			.finally(() => (loading = false));
	}

	onMount(() => refresh());
</script>

<article class="dark:bg-gray-800 dark:text-gray-400 bg-gray-300 text-black">
	<div class="p-4 flex flex-col h-full min-h-screen">
		<div class="p-2 flex flex-row items-center justify-between">
			<div>
        <a class="flex flex-row" href="/">
          <img class="inline w-12 h-12 dark:mix-blend-luminosity brightness-150" alt="PhysLib" src={logo} />
          <span class="pl-2 flex flex-col justify-center">
            <span class="text-center">Phys</span>
            <span class="text-center">Lib</span>
          </span>
        </a>
      </div>
			<div class="w-2/3 flex flex-row min-[0px]:max-sm:hidden">
				<div class="w-full">
					<SearchInput bind:searchText on:onSearch={refresh} />
				</div>
			</div>
			<div class="flex flex-col justify-center">
        <ThemeToggle/>
      </div>
		</div>
    <div class="p-2 sm:hidden">
      <div class="flex flex-row">
        <div class="w-full">
          <SearchInput bind:searchText on:onSearch={refresh} />
        </div>
      </div>
    </div>
		<div class="">
      {#if result?.works}
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 xl:grid-cols-4">
          {#each result.works as work}
            <WorkCard {work} {result} />
          {/each}
        </div>
      {/if}
			{#if !loading && result?.works?.length == 0}
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
