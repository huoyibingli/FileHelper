public class Singleton
    {
        private static Singleton instance;
        private static readonly object SyncRoot = new object();          ///程序运行时创建一个静态的只读对象
        private Singleton(){}
        public static Singleton GetInstance()
        {
            ///双重锁定   先判断实例是否存在，不存在再加锁处理
            ///这样不用让线程每次都加锁，保证了线程安全，也提高了性能
            if (instance == null)   
            {
                lock (SyncRoot)   ///在同一个程序加了锁的那部分程序只有一个线程可以加入
                {
　　　　　　　　　　　　///若实例不存在，则new一个新实例，否则返回已有的实例　　　　　　
                    if (instance == null)
                    {
                        instance = new Singleton();
                    }
                }
            }
            return instance;
        }
    }