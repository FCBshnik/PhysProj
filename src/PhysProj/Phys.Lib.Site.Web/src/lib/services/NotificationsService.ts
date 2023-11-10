import * as api from '$lib/services/ApiService';
import { createNanoEvents } from 'nanoevents'

interface NotificationsEvents {
    message: (message:string | undefined) => void
}

const emitter = createNanoEvents<NotificationsEvents>()

class NotificationsService {
    public push(message:any) {
        if (message instanceof api.ErrorModel) {
            var errorModel = message as api.ErrorModel;
            console.info('api error:', errorModel);
            emitter.emit('message', errorModel.message)
        }
        else {
            console.warn("message:", message);
            emitter.emit('message', message)
        }
    }

    on<E extends keyof NotificationsEvents>(event: E, callback: NotificationsEvents[E]) {
        return emitter.on(event, callback)
    }
}

let service = new NotificationsService();
export { service as notificationsService };