import json, math, random
from http.server import BaseHTTPRequestHandler, HTTPServer

state = {}
def reset():
    state.update(target=(random.uniform(-.6,.6), random.uniform(.25,.9), random.uniform(-.2,.2)), temperature=0.0, throws=0, hits=0)
    return state
def observe():
    x,y,z=state["target"]
    return {"target_relative":[round(x,4),round(y,4),round(z,4)],"temperature":round(state["temperature"],4),"throws_remaining":20-state["throws"]}
def throw(a):
    h=float(a.get("horizontal",0)); v=float(a.get("vertical",.5)); p=float(a.get("power",.65)); x,y,z=state["target"]
    error=math.sqrt((h-x/.6)**2+(v-y)**2+(p-(.45+.25*y))**2)
    reward=10.0 if error < .22 else -1.0+max(0.0,.9-error)
    state["throws"]+=1
    if reward>=10: state["hits"]+=1; state["temperature"]+=.1
    return {"reward":round(reward,4),"hit":reward>=10,"observation":observe(),"done":state["throws"]>=20}
class Handler(BaseHTTPRequestHandler):
    def do_POST(self):
        n=int(self.headers.get("Content-Length",0)); body=json.loads(self.rfile.read(n) or b"{}")
        method=body.get("method",""); params=body.get("params",{}) or {}
        if method in ("initialize","notifications/initialized"): result={"protocolVersion":"2024-11-05","capabilities":{"tools":{}},"serverInfo":{"name":"sauna","version":"1.0"}}
        elif method=="tools/list": result={"tools":[{"name":"reset","description":"Start a fresh episode.","inputSchema":{"type":"object"}},{"name":"observe","description":"Read state.","inputSchema":{"type":"object"}},{"name":"throw","description":"Throw a projectile.","inputSchema":{"type":"object","properties":{"horizontal":{"type":"number"},"vertical":{"type":"number"},"power":{"type":"number"}},"required":["horizontal","vertical","power"]}}]}
        elif method=="tools/call":
            name=params.get("name"); args=params.get("arguments",{}); value=reset() if name=="reset" else observe() if name=="observe" else throw(args); result={"content":[{"type":"text","text":json.dumps(value)}]}
        else: result={}
        out=json.dumps({"jsonrpc":"2.0","id":body.get("id"),"result":result}).encode(); self.send_response(200); self.send_header("Content-Type","application/json"); self.send_header("Content-Length",str(len(out))); self.end_headers(); self.wfile.write(out)
    def log_message(self,*args): pass
reset(); HTTPServer(("0.0.0.0",8000),Handler).serve_forever()
