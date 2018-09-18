﻿using System;
using System.Diagnostics;
using System.IO;

namespace Saplin.StorageSpeedMeter
{
    public class MemCopyTest : SequentialTest
    {
        private int[] src, dst;
        int current = 0;

        public MemCopyTest(FileStream file, int blockSize, long totalBlocks = 0) : base(file, blockSize, totalBlocks)
        {
        }

        public override string DisplayName { get => "Memory copy" + " [" + blockSize / 1024 / 1024 + "MB] block"; }

        protected override void DoOperation(byte[] buffer, Stopwatch sw)
        {
            var rand = new Random();

            sw.Restart();
            Buffer.BlockCopy(src, 0, dst, current, src.Length);
            sw.Stop();
            current += blockSize / sizeof(int);
            if (current >= dst.Length)
            {
                current = 0;
                src[0] = rand.Next();
                src[1] = rand.Next();
                src[src.Length-1] = rand.Next();
            }
        }

        protected override byte[] InitBuffer()
        {
            Status = TestStatus.InitMemBuffer;

            src = new int[blockSize / sizeof(int)];
            dst = new int[(blockSize * (totalBlocks / (Environment.Is64BitProcess ? 4 : 4*4))) / sizeof(int)];

            var rand = new Random();

            for (int i = 0; i < src.Length; i++)
                src[i] = rand.Next();

            current = 0;
            return null;
        }

        protected override void TestCompleted()
        {
            src = null;
            dst = null;

            base.TestCompleted();
        }
    }
}
