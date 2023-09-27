<script lang="ts">
	import * as api from '$lib/services/ApiService';
	import { goto } from '$app/navigation';
	import Footer from '$lib/components/Footer.svelte';

	if (!api.service.isAuthorized()) goto('/login');

	function logout() {
		api.service.clearToken();
		goto('/login');
	}
</script>

<div class="flex flex-col h-screen">
  <nav>
    <a href="/home">PhysLibAdmin</a>
    <ul>
      <a href="/migrations">Migrations</a>
      <a href="/authors">Authors</a>
      <a href="/works">Works</a>
      <a href="/files">Files</a>
    </ul>
    <ul>
      <button on:click={logout}>Logout</button>
    </ul>
  </nav>
  <div class="flex-grow">
    <slot />
  </div>
	<div>
		<Footer />
	</div>
</div>
