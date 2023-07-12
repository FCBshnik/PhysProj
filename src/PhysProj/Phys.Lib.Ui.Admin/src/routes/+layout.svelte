<script lang="ts">
  import "../app.css";

  import * as api from '$lib/services/ApiService';
  import { goto } from '$app/navigation';
  import Notifications from "$lib/components/Notifications.svelte";

  if (!api.service.isAuthorized())
    goto("/login");
</script>

<Notifications/>
<slot />