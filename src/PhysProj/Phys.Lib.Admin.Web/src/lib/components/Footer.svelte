<script lang="ts">
    import {onMount, onDestroy} from 'svelte';
    import * as api from '$lib/services/ApiService';

    let apiVersion:string|undefined;
    let apiTime:Date|undefined;
    let isOnline:boolean = false;
    let checkTimeout:number;

    $: onlineClass = isOnline ? 'text-green-900' : 'text-red-900';

    function check() {
        api.service.healthCheck()
            .then(r => {
                apiVersion = r.version;
                apiTime = r.time;
                isOnline = true;
            })
            .catch(e => { isOnline = false; });
    }

    onMount(() => {
      check() 
      checkTimeout = setInterval(check, 10000)
    });
    onDestroy(() => clearTimeout(checkTimeout));
</script>

<div class="p-4 text-right {onlineClass}">
    {isOnline} web. ver 2023.07.25 api ver. {apiVersion}, time {apiTime?.toISOString()}
</div>