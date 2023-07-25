<script lang="ts">
    import * as api from '$lib/services/ApiService';

    let apiVersion:string|undefined;
    let apiTime:Date|undefined;

    function check() {
        api.service.healthCheck()
            .then(r => {
                apiVersion = r.version;
                apiTime = r.time
            });
    }

    check();
    setInterval(check, 10000);
</script>

<div class="absolute bottom-0 right-0 max-w-md w-full p-4 text-right">
    api ver. {apiVersion}, time {apiTime?.toISOString()}
</div>