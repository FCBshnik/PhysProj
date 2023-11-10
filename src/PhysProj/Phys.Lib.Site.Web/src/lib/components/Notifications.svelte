<script lang="ts">
    import { notificationsService } from "$lib/services/NotificationsService"

    var message:string|undefined = undefined
    var timeout:number = 0

    notificationsService.on('message', m => {
        if (timeout)
            clearTimeout(timeout)
        message = m
        timeout = setTimeout(close, 10000)
    })

    function close(){
      message = undefined;
    }
</script>

{#if message}
    <button on:click={close} class="absolute top-5 right-5 max-w-md w-full p-4 bg-gray-300 text-gray-600 bg-opacity-80 rounded-md">
        {message}
    </button>
{/if}