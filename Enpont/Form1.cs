using System;
using System.IO;
using System.Globalization;
using System.Windows.Forms;

namespace Enpont
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.KeyPress += TextBox_KeyPress;
            textBox2.KeyPress += TextBox_KeyPress;
            textBox3.KeyPress += TextBox_KeyPress;
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            char inputChar = e.KeyChar;
            if (!IsHexadecimalChar(inputChar) && !Char.IsControl(inputChar))
            {
                e.Handled = true;
            }
        }

        private bool IsHexadecimalChar(char inputChar)
        {
            return (inputChar >= '0' && inputChar <= '9') || (inputChar >= 'A' && inputChar <= 'F') || (inputChar >= 'a' && inputChar <= 'f');
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Todos os arquivos|*.*";
            openFileDialog.Title = "Selecione um arquivo";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;

                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(stream);

                    FileInfo fileInfo = new FileInfo(filePath);

                    long tamanhoarquivo = fileInfo.Length;

                    long offset1 = long.Parse(textBox1.Text, NumberStyles.HexNumber);
                    long offset2 = long.Parse(textBox2.Text, NumberStyles.HexNumber);
                    long offset3 = long.Parse(textBox3.Text, NumberStyles.HexNumber);

                    long sub1 = offset2 - offset1;
                    long sub2 = offset3 - offset2;
                    long sub3 = offset3 - offset1;

                    long offsetponteiro1 = 0;
                    long offsetponteiro2 = 0;
                    long offsetponteiro3 = 0;

                    bool foundOffsets = false;

                    if (radioButton32Bits.Checked)
                    {
                        var result = MessageBox.Show("O arquivo é Little Endian?", "Seleção de Endian", MessageBoxButtons.YesNoCancel);

                        bool isLittleEndian = false;
                        if (result == DialogResult.Yes)
                        {
                            isLittleEndian = true;
                        }
                        else if (result == DialogResult.No)
                        {
                            isLittleEndian = false;
                        }
                        else
                        {
                            return;
                        }

                        for (long currentPosition = 0; currentPosition < tamanhoarquivo - 4; currentPosition += 1)
                        {
                            br.BaseStream.Position = currentPosition;

                            int valor1;

                            if (isLittleEndian)
                            {
                                valor1 = br.ReadInt32();
                            }
                            else
                            {
                                byte[] bytes1 = br.ReadBytes(4);
                                Array.Reverse(bytes1);
                                valor1 = BitConverter.ToInt32(bytes1, 0);
                            }

                            if (valor1 == offset1)
                            {
                                offsetponteiro1 = currentPosition;

                                for (long currentPosition2 = offsetponteiro1 + 4; currentPosition2 < tamanhoarquivo - 4; currentPosition2 += 1)
                                {
                                    br.BaseStream.Position = currentPosition2;

                                    int valor2;

                                    if (isLittleEndian)
                                    {
                                        valor2 = br.ReadInt32();
                                    }
                                    else
                                    {
                                        byte[] bytes2 = br.ReadBytes(4);
                                        Array.Reverse(bytes2);
                                        valor2 = BitConverter.ToInt32(bytes2, 0);
                                    }

                                    if (valor2 == offset2)
                                    {
                                        offsetponteiro2 = currentPosition2;

                                        for (long currentPosition3 = offsetponteiro2 + 4; currentPosition3 < tamanhoarquivo - 4; currentPosition3 += 1)
                                        {
                                            br.BaseStream.Position = currentPosition3;

                                            int valor3;

                                            if (isLittleEndian)
                                            {
                                                valor3 = br.ReadInt32();
                                            }
                                            else
                                            {
                                                byte[] bytes3 = br.ReadBytes(4);
                                                Array.Reverse(bytes3);
                                                valor3 = BitConverter.ToInt32(bytes3, 0);
                                            }

                                            if (valor3 == offset3)
                                            {
                                                // Encontrou o terceiro ponteiro
                                                offsetponteiro3 = currentPosition3;

                                                MessageBox.Show("Possíveis offsets dos ponteiros:\n\nOffset primeiro ponteiro: " + offsetponteiro1.ToString("X2") + "\nOffset segundo ponteiro: " + offsetponteiro2.ToString("X2") + "\nOffset terceiro ponteiro: " + offsetponteiro3.ToString("X2"));

                                                foundOffsets = true;
                                                break;
                                            }
                                        }
                                        if (foundOffsets)
                                            break;
                                    }
                                }
                                if (foundOffsets)
                                    break;
                            }
                        }
                        if (!foundOffsets)
                        {
                            MessageBox.Show("Infelizmente não encontrei os offsets dos ponteiros em 32 bits!");
                        }
                    }

                    if (radioButton16Bits.Checked)
                    {
                        var result = MessageBox.Show("O arquivo é Little Endian?", "Seleção de Endian", MessageBoxButtons.YesNoCancel);

                        bool isLittleEndian = false;
                        if (result == DialogResult.Yes)
                        {
                            isLittleEndian = true;
                        }
                        else if (result == DialogResult.No)
                        {
                            isLittleEndian = false;
                        }
                        else
                        {
                            return;
                        }

                        bool foundOffsets16Bits = false;

                        for (long currentPosition16Bits = 0; currentPosition16Bits < tamanhoarquivo - 2; currentPosition16Bits += 1)
                        {
                            br.BaseStream.Position = currentPosition16Bits;

                            short valor16Bits;

                            if (isLittleEndian)
                            {
                                valor16Bits = br.ReadInt16();
                            }
                            else
                            {
                                byte[] bytes16Bits = br.ReadBytes(2);
                                Array.Reverse(bytes16Bits);
                                valor16Bits = BitConverter.ToInt16(bytes16Bits, 0);
                            }

                            if (valor16Bits == offset1)
                            {
                                offsetponteiro1 = currentPosition16Bits;

                                for (long currentPosition216Bits = offsetponteiro1 + 2; currentPosition216Bits < tamanhoarquivo - 2; currentPosition216Bits += 1)
                                {
                                    br.BaseStream.Position = currentPosition216Bits;

                                    short valor216Bits;

                                    if (isLittleEndian)
                                    {
                                        valor216Bits = br.ReadInt16();
                                    }
                                    else
                                    {
                                        byte[] bytes216Bits = br.ReadBytes(2);
                                        Array.Reverse(bytes216Bits);
                                        valor216Bits = BitConverter.ToInt16(bytes216Bits, 0);
                                    }

                                    if (valor216Bits == offset2)
                                    {
                                        offsetponteiro2 = currentPosition216Bits;

                                        for (long currentPosition316Bits = offsetponteiro2 + 2; currentPosition316Bits < tamanhoarquivo - 2; currentPosition316Bits += 1)
                                        {
                                            br.BaseStream.Position = currentPosition316Bits;

                                            short valor316Bits;

                                            if (isLittleEndian)
                                            {
                                                valor316Bits = br.ReadInt16();
                                            }
                                            else
                                            {
                                                byte[] bytes316Bits = br.ReadBytes(2);
                                                Array.Reverse(bytes316Bits);
                                                valor316Bits = BitConverter.ToInt16(bytes316Bits, 0);
                                            }

                                            if (valor316Bits == offset3)
                                            {
                                                offsetponteiro3 = currentPosition316Bits;

                                                MessageBox.Show("Possíveis offsets dos ponteiros:\n\nOffset primeiro ponteiro: " + offsetponteiro1.ToString("X2") + "\nOffset segundo ponteiro: " + offsetponteiro2.ToString("X2") + "\nOffset terceiro ponteiro: " + offsetponteiro3.ToString("X2"));

                                                foundOffsets16Bits = true;
                                                break;
                                            }
                                        }
                                        if (foundOffsets16Bits)
                                            break;
                                    }
                                }
                                if (foundOffsets16Bits)
                                    break;
                            }
                        }
                        if (!foundOffsets16Bits)
                        {
                            MessageBox.Show("Infelizmente não encontrei os offsets dos ponteiros em 16 bits!");
                        }
                    }
                }
            }
        }
    }
}
