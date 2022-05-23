import { Observable, Observer } from 'rxjs';
import { AnonymousSubject } from 'rxjs/internal/Subject';
import { Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { RequestType } from "src/app/types/enums";

export interface ServerResponse {
  type: string;
}

export class WebsocketService {
  private subject: AnonymousSubject<MessageEvent>;
  public messages: Subject<any>;

  constructor(host: string, port: number) {
    this.subject = this.connect(host, port);
    this.messages = <Subject<ServerResponse>>this.subject.pipe(
      map(
        (response: MessageEvent): ServerResponse => {
          console.log(response.data);
          let data = JSON.parse(response.data)
          return data;
        }
      ));
  }

  public connect(host: string, port: number): AnonymousSubject<MessageEvent> {
    if (!this.subject) {
      this.subject = this.create(`ws://${host}:${port}`);
    }
    return this.subject;
  }

  public login(username: string, password: string) {
    this.messages.next({ username, password, type: RequestType.Authenticate });
  }

  private create(url: string): AnonymousSubject<MessageEvent> {
    let ws = new WebSocket(url);
    let observable = new Observable((obs: Observer<MessageEvent>) => {
      ws.onmessage = obs.next.bind(obs);
      ws.onerror = obs.error.bind(obs);
      ws.onclose = obs.complete.bind(obs);
      return ws.close.bind(ws);
    });
    let observer = {
      error: () => { },
      complete: () => { },
      next: (data: Object) => {
        console.log('Message sent to websocket: ', data);
        if (ws.readyState === WebSocket.OPEN) {
          ws.send(JSON.stringify(data));
        }
      }
    };
    return new AnonymousSubject<MessageEvent>(observer, observable);
  }
}