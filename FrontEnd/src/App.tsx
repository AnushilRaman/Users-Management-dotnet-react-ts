import { Toaster } from "react-hot-toast";
import GlobalRouter from "./routes";

export default function App() {
  return (
    <>
      <GlobalRouter />
      <Toaster />
    </>
  )
}