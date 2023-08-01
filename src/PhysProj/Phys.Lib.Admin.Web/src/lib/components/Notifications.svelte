<script lang="ts">
    import { notificationsService } from "$lib/services/NotificationsService"

    var message:string|undefined = undefined
    var timeout:number = 0;

    notificationsService.on('message', m => {
        if (timeout)
            clearTimeout(timeout);
        message = m
        timeout = setTimeout(() => { message = undefined }, 5000);
    })
</script>

{#if message}
    <div class="absolute top-0 right-0 max-w-md w-full p-4 bg-gray-100 border border-gray-100 text-sm text-gray-800 rounded-md shadow-md">
        {message}
    </div>
{/if}