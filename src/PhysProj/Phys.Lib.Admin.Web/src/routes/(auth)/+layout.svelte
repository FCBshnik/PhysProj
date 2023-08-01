<script lang="ts">
	import * as api from '$lib/services/ApiService';
	import { goto } from '$app/navigation';

	if (!api.service.isAuthorized()) goto('/login');

	function logout() {
		api.service.clearToken();
		goto('/login');
	}
</script>

<nav>
	<a href="/home">PhysLibAdmin</a>
	<ul>
		<a href="/authors">Authors</a>
		<a href="/works">Works</a>
		<a href="/files">Files</a>
	</ul>
	<ul>
		<button on:click={logout}>Logout</button>
	</ul>
</nav>

<slot />
