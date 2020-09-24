using System;
using System.Collections.Generic;
using System.Linq;

namespace FiniteStateMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            var r1 = TraverseStates(new[] { "APP_ACTIVE_OPEN", "RCV_SYN_ACK", "RCV_FIN" });
            var r2 = TraverseStates(new[] { "APP_PASSIVE_OPEN", "RCV_SYN", "RCV_ACK" });
            var r3 = TraverseStates(new[] { "APP_ACTIVE_OPEN", "RCV_SYN_ACK", "RCV_FIN", "APP_CLOSE" });
            var r4 = TraverseStates(new[] { "APP_ACTIVE_OPEN" });
            var r5 = TraverseStates(new[] { "APP_PASSIVE_OPEN", "RCV_SYN", "RCV_ACK", "APP_CLOSE", "APP_SEND" });

            Console.WriteLine("Hello World!");
        }

        public class state
        {
            public state(string key, string parent)
            {
                this.key = key;
                this.parent = parent;
            }

            public string key;
            public string parent;
        }

        public static string TraverseStates(string[] events)
        {
            var state = "CLOSED"; // initial state, always
                                  // Your code here!
            Dictionary<state, object> states_CLOSED = new Dictionary<state, object>();
            Dictionary<state, object> states_FIN_WAIT_1 = new Dictionary<state, object>();
            Dictionary<state, object> states_SYN_SENT = new Dictionary<state, object>();

            Dictionary<state, object> states_LAST_ACK = new Dictionary<state, object>();
            states_LAST_ACK.Add(new state("RCV_ACK", "LAST_ACK"), states_CLOSED);

            Dictionary<state, object> states_TIME_WAIT = new Dictionary<state, object>();
            states_TIME_WAIT.Add(new state("APP_TIMEOUT", "TIME_WAIT"), states_CLOSED);

            Dictionary<state, object> states_CLOSING = new Dictionary<state, object>();
            states_CLOSING.Add(new state("RCV_ACK", "CLOSING"), states_TIME_WAIT);

            Dictionary<state, object> states_FIN_WAIT_2 = new Dictionary<state, object>();
            states_FIN_WAIT_2.Add(new state("RCV_FIN", "FIN_WAIT_2"), states_TIME_WAIT);

            Dictionary<state, object> states_CLOSE_WAIT = new Dictionary<state, object>();
            states_CLOSE_WAIT.Add(new state("APP_CLOSE", "CLOSE_WAIT"), states_LAST_ACK);

            Dictionary<state, object> states_ESTABLISHED = new Dictionary<state, object>();
            states_ESTABLISHED.Add(new state("RCV_FIN", "ESTABLISHED"), states_CLOSE_WAIT);
            states_ESTABLISHED.Add(new state("APP_CLOSE", "ESTABLISHED"), states_FIN_WAIT_1);

            states_FIN_WAIT_1.Add(new state("RCV_FIN", "FIN_WAIT_1"), states_CLOSING);
            states_FIN_WAIT_1.Add(new state("RCV_FIN_ACK", "FIN_WAIT_1"), states_TIME_WAIT);
            states_FIN_WAIT_1.Add(new state("RCV_ACK", "FIN_WAIT_1"), states_FIN_WAIT_2);

            Dictionary<state, object> states_SYN_RCVD = new Dictionary<state, object>();
            states_SYN_RCVD.Add(new state("APP_CLOSE", "SYN_RCVD"), states_FIN_WAIT_1);
            states_SYN_RCVD.Add(new state("RCV_ACK", "SYN_RCVD"), states_ESTABLISHED);

            Dictionary<state, object> states_LISTEN = new Dictionary<state, object>();
            states_LISTEN.Add(new state("RCV_SYN", "LISTEN"), states_SYN_RCVD);
            states_LISTEN.Add(new state("APP_SEND", "LISTEN"), states_SYN_SENT);
            states_LISTEN.Add(new state("APP_CLOSE", "LISTEN"), states_CLOSED);


            states_SYN_SENT.Add(new state("RCV_SYN", "SYN_SENT"), states_ESTABLISHED);
            states_SYN_SENT.Add(new state("RCV_SYN_ACK", "SYN_SENT"), states_ESTABLISHED);
            states_SYN_SENT.Add(new state("APP_CLOSE", "SYN_SENT"), states_ESTABLISHED);

            states_CLOSED.Add(new state("APP_PASSIVE_OPEN", "CLOSED"), states_LISTEN);
            states_CLOSED.Add(new state("APP_ACTIVE_OPEN", "CLOSED"), states_SYN_SENT);

            Func<object, string, object> asd = (a, b) =>
            {

                var tt = (a as Dictionary<state, object>).Where(z => z.Key.key.Equals(b)).SingleOrDefault().Value;
                return tt;
            };

            object acum = states_CLOSED;
            var t = events.ToList().Aggregate(acum, asd);

            if (t == null)
            {
                state = "ERROR";
            }
            else
            {
                state = (t as Dictionary<state, object>).First().Key.parent;
            }

            return state;
        }
    }
}
