<script lang="ts">
  import * as api from '$lib/services/ApiService';
  import { goto } from '$app/navigation';

  if (api.service.isAuthorized())
    goto("/home");
  else
    goto("/login");
</script>

<slot />