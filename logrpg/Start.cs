using System;
using System.Collections.Generic;
using logrpg.UserAuth;

namespace logrpg {
    internal class Start {
        public static void Main(
            string[] args
        ) {
            DataHandle DH = DataHandle.getHandle(
                "server=192.168.56.101;" +
                "uid=logrpg;" +
                "pwd=logrpg;" +
                "database=logrpg;" +
                "Allow User Variables=True;"
            );
            DH.Auth ("kevok","logrpg");
        }
    }
}
