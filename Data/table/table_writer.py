#!/usr/bin/python
#coding=GBK

#打表工具

import sys
import xlrd
import re
import google

sys.path.append("../tableProto")

#import table_common_pb2

# 行游标，为了追踪在哪行挂掉的
curr_row=0

ENUM_FIELD_NAME = 0
ENUM_FIELD_NUMBER = 1
ENUM_FIELD_TYPE = 2
ENUM_FIELD_LABEL = 3

ENABLE_VERBOSE = False
#ENABLE_VERBOSE = True
DUMP_CLIENT = False

def INFO(msg):
    print "[INFO] " + str(msg)

def ERROR(msg):
    msg = str(msg)
    print "[INFO] " + str(msg)
    exit(0)

def TRACE(msg):
    if ENABLE_VERBOSE:
        print "[TRACE] " + str(msg)

# Usage
def Usage():
    print "%s [-c(clien mode)|-v(verbose)] test(use: test_pb2, test.xls, test.tbl)" % sys.argv[0]


##########################
def ParseRange(row_value):
    pattern = '^[\[\(]{1}[0-9]+,[0-9]+[\]\)]'
    if not re.match(pattern, row_value):
        print row_value
        ERROR("invalid range format: " + row_value )
        return None

    range_value = table_common_pb2.Range()
    range_value.range_type = range_value.RANGE_TYPE_INVALID
    if row_value[0] == '[' and row_value[-1] == ']':
        range_value.range_type = range_value.RANGE_TYPE_CLOSED_CLOSED
    elif row_value[0] == '[' and row_value[-1] == ')':
        range_value.range_type = range_value.RANGE_TYPE_CLOSED_OPEN
    elif row_value[0] == '(' and row_value[-1] == ']':
        range_value.range_type = range_value.RANGE_TYPE_OPEN_CLOSED
    elif row_value[0] == '(' and row_value[-1] == ')':
        range_value.range_type = range_value.RANGE_TYPE_OPEN_OPEN
    else:
        ERROR("what happens: " + row_value)
        exit(1)

    pair = row_value[1:-1].split(',')
    range_value.lhs = int(pair[0])
    range_value.rhs = int(pair[1])
    return range_value

##########################
def ParseIntPair(row_value):
    INFO("int_pair = " + row_value)
    TRACE("----------------一对数字a-----------------------")
    INFO("int_pair = " + row_value)
    pair = table_common_pb2.IntPair()
    list = row_value.split('/')
    lenght = int(len(list))
    if lenght >= 1:
        pair.molecule = int(list[0])
    if lenght >= 2:
        pair.denominator = int(list[1])
    return pair
def ParseIntPairList(row_value):
    pattern = '/^([0-9]*,[0-9]*^\^)*([0-9]*,[0-9]*)$'
    if not re.match(pattern, row_value):
        ERROR("invalid IntPairList format: " + row_value)
        return None

    int_pair_list = table_common_pb2.IntPairList()

    pair_list = row_value.split('^')
    for data in pair_list:
        strs = data.split('/')
        int_pari = int_pair_list.list.add()
        pair = ParseIntPair(strs)
        int_pari.append(pair)

    return int_pair_list
def ParseIntTupleList(row_value):
    pattern = '^([0-9]*,[0-9]*,[0-9]*\^)*([0-9]*,[0-9]*,[0-9]*)$'
    if not re.match(pattern, row_value):
        ERROR("invalid IntTupleList format: " + row_value)
        return None

    int_tuple_list = table_common_pb2.IntTupleList()

    tuple_list = row_value.split('^')
    for _tuple in tuple_list:
        __tuple = _tuple.split(',')
        int_tuple = int_tuple_list.list.add()
        int_tuple.first = int(__tuple[0])
        int_tuple.second = int(__tuple[1])
        int_tuple.third = int(__tuple[2])

    return int_tuple_list
def ParseIntList(row_value):
    int_list = table_common_pb2.IntList()
    list = row_value.split('/')
    for v in list:
        int_list.list.append(int(v))
    return int_list
##### New Struct #####
def ParseIntListList(row_value):
    '''pattern = '^([0-9]*,[0-9]*,[0-9]*\^)*([0-9]*,[0-9]*,[0-9]*)$'
    if not re.match(pattern, row_value):
        ERROR("invalid IntListList format: " + row_value)
        return None'''
    int_list_list = table_common_pb2.IntListList()
    list_list = row_value.split('/')
    for f_list in list_list:
        internal_list = int_list_list.list.add()
        int_list = f_list.split('#')
        for v in int_list:
            internal_list.list.append(int(v))
    return int_list_list
def ParseIntListJingHao(row_value):
    int_list = table_common_pb2.IntListJingHao()
    list = row_value.split('#')
    for v in list:
        int_list.list.append(int(v))
    return int_list
def ParseIntListXiaHuaXian(row_value):
    int_list = table_common_pb2.IntListXiaHuaXian()
    list = row_value.split('_')
    for v in list:
        int_list.list.append(int(v))
    return int_list
def ParseIntListJingHaoMeiYuan(row_value):
    int_list_list = table_common_pb2.IntListJingHaoMeiYuan()
    list_list = row_value.split('&')
    for f_list in list_list:
        internal_list = int_list_list.list.add()
        int_list = f_list.split('#')
        for v in int_list:
            internal_list.list.append(int(v))
    return int_list_list
def ParseFenHao(row_value):
    int_list_list = table_common_pb2.IntListXiaHuaXianFenHao()
    list_list = row_value.split(';')
    for f_list in list_list:
        internal_list = int_list_list.list.add()
        int_list = f_list.split('_')
        for v in int_list:
            internal_list.list.append(int(v))
    return int_list_list
def ParseStringList(row_value):
    int_list = table_common_pb2.StringList()
    list = row_value.split('#')
    for v in list:
        int_list.list.append(v)
    return int_list
def ParseIntSpecXiaHuaJinHao(row_value):
    int_list = table_common_pb2.IntSpecXiaHuaJinHao()
    list = row_value.split('#')
    index = 0
    for v in list:
        if index==0:
            list1 = v.split('_')
            int_list.value0 = int(list1[0])
            int_list.list.append(int(list1[1]))
        else:
            int_list.list.append(int(v))
        index=index+1
    return int_list
##### main process #####
def Process(workbook_name, sheet_name, entry_name):
    xls_file =  workbook_name + ".xls"

    if DUMP_CLIENT:
        tbl_file = (workbook_name + ("" if sheet_name == None else "_" + sheet_name)).lower() + ".bytes"
    else:
        tbl_file = (workbook_name + ("" if sheet_name == None else "_" + sheet_name)).lower() + ".tbl"

    if entry_name == None:
        entry_name = (workbook_name + ("" if sheet_name == None else "_" + sheet_name)).upper()

    array_name = entry_name + "ARRAY"

    if DUMP_CLIENT:
        pb2 = "c_table_" + workbook_name.lower() + "_pb2"
    else:
        pb2 = "table_" + workbook_name.lower() + "_pb2"

    print "=========================================================="
    print "dumping table ... \"" + xls_file + "\""
    print "=========================================================="

    INFO("workbook_name = " + workbook_name)
    INFO("sheet_name = " + "" if sheet_name == None else sheet_name)
    INFO("xls_file = " + xls_file)
    INFO("tbl_file = " + tbl_file)
    INFO("entry_name = " + entry_name)
    INFO("array_name = " + array_name)
    INFO("pb2 = " + pb2)
    module = __import__(pb2)

    proto_desc = []

    # alias FieldDescriptor
    FieldDescriptor = google.protobuf.descriptor.FieldDescriptor

    for desc in getattr(module, entry_name).DESCRIPTOR.fields:
        # 只读前99列，后面的可以作为后处理字段使用
        #if desc.number > 99:
        #    continue;

        #if not (((desc.label == desc.LABEL_REQUIRED or desc.label == desc.LABEL_REPEATED) and desc.type != desc.TYPE_MESSAGE) or \
        #        (desc.type == desc.TYPE_MESSAGE and desc.label == desc.LABEL_OPTIONAL)):
        #    if(desc.type == desc.TYPE_MESSAGE):
        #        ERROR("jiabao:" + desc.name + '\'s label can not be others unless \'optional\'')
        #    else:
        #        ERROR("jiabao:" + desc.name + '\'s label should not be \'optional\' just be \'required or repeated\'')
        #    continue;

        field_name = desc.name

        # post operated field, start with '_'
        if field_name[0] == '_':
            continue;

        field_number = desc.number
        field_label = desc.label
        field_type = None
        if desc.type == desc.TYPE_INT32 or \
                desc.type == desc.TYPE_INT64 or \
                desc.type == desc.TYPE_SINT32 or \
                desc.type == desc.TYPE_SINT64 or \
                desc.type == desc.TYPE_FIXED32 or \
                desc.type == desc.TYPE_FIXED64 or \
                desc.type == desc.TYPE_UINT32 or \
                desc.type == desc.TYPE_UINT64 or \
                desc.type == desc.TYPE_ENUM :
            field_type = int
        elif desc.type == desc.TYPE_BOOL:
            field_type = bool
        elif desc.type == desc.TYPE_FLOAT:
            field_type = float
        elif desc.type == desc.TYPE_BYTES or \
                desc.type == desc.TYPE_STRING:
            field_type = str
        

        proto_desc += [(field_name, field_number, field_type, field_label)]

    TRACE("proto_desc = " + str(proto_desc))

    book = xlrd.open_workbook(xls_file)
    sheet = book.sheet_by_index(0) if sheet_name == None else book.sheet_by_name(sheet_name)
    row_array = getattr(module, array_name)()
    INFO("Sheet Rows: " + str(sheet.nrows))

    global curr_row
    for curr_row in xrange(sheet.nrows):
        if curr_row < 1:   # first line is desc
            continue

        row_values = sheet.row_values(curr_row)
        if row_values[0] == "RESTRAINT":
            INFO("RESTRAINT at row " + str(curr_row))
            continue
        elif row_values[0] == "HEADER":
            INFO("HEADER at row " + str(curr_row));
            continue;

        TRACE("row_values = " + str(row_values))

        row = row_array.rows.add()
        for field_desc in proto_desc:
            TRACE(field_desc)

            row_value = row_values[field_desc[ENUM_FIELD_NUMBER]-1]
            TRACE("SSS" + str(field_desc[ENUM_FIELD_NUMBER]-1))
            TRACE(str(row))
            TRACE("---------------1------------------------")
            if type(row_value) == float:
                row_value = str(int(row_value))

            # 字符串
            if field_desc[ENUM_FIELD_TYPE] == str:
                if field_desc[ENUM_FIELD_LABEL] == FieldDescriptor.LABEL_REQUIRED:
                    setattr(row, field_desc[ENUM_FIELD_NAME], row_value)
                elif field_desc[ENUM_FIELD_LABEL] == FieldDescriptor.LABEL_REPEATED:
                    for section in row_value.strip().replace('|', '^').split('^'):
                        section = section.strip()
                        if section != "":
                            getattr(row, field_desc[ENUM_FIELD_NAME]).append(section)
                else:
                    ERROR("invalid laber")
                    exit(1)

            # 整型
            elif field_desc[ENUM_FIELD_TYPE] == int:
                if field_desc[ENUM_FIELD_LABEL] == FieldDescriptor.LABEL_REQUIRED:
                    #TRACE(row_value)
                    row_value = int(0) if row_value == "" else int(row_value)
                    setattr(row, field_desc[ENUM_FIELD_NAME], row_value)
                elif field_desc[ENUM_FIELD_LABEL] == FieldDescriptor.LABEL_REPEATED:
                    for section in row_value.strip().replace('|', '^').split('^'):
                        section = section.strip()
                        if section != "":
                            getattr(row, field_desc[ENUM_FIELD_NAME]).append(int(section))

                else:
                    ERROR("invalid laber")
                    exit(1)

            # 布尔
            elif field_desc[ENUM_FIELD_TYPE] == bool:
                if field_desc[ENUM_FIELD_LABEL] == FieldDescriptor.LABEL_REQUIRED:
                    row_value = int(0) if row_value == "" else int(row_value)
                    setattr(row, field_desc[ENUM_FIELD_NAME], bool(row_value))
                elif field_desc[ENUM_FIELD_LABEL] == FieldDescriptor.LABEL_REPEATED:
                    for section in row_value.strip().replace('|', '^').split('^'):
                        if section != "":
                            getattr(row, field_desc[ENUM_FIELD_NAME]).append(bool(section))
                else:
                    ERROR("invalid laber")
                    exit(1)
            #float
            elif field_desc[ENUM_FIELD_TYPE] == float:
                if field_desc[ENUM_FIELD_LABEL] == FieldDescriptor.LABEL_REQUIRED:
                    row_value = float(0) if row_value == "" else float(row_value)
                    setattr(row,field_desc[ENUM_FIELD_NAME],float(row_value))
                elif field_desc[ENUM_FIELD_LABEL] == FieldDescriptor.LABEL_REPEATED:
                    for section in row_value.strip().replace('|','^').split('^'):
                        if section != "":
                            getattr(row,field_desc[ENUM_FIELD_NAME].append(float(section)))
                else:
                    ERROR("invalid laber ")
                    exit(1)

            
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.ActivityAwardList:
                if row_value != "":
                    item_drop = ParseActivityAwardList(row_value)
                    if not item_drop:
                        ERROR("pass item drop failed: " + row_value)
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(item_drop)

            # 区间
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.Range:
                if row_value != "":
                    range_value = ParseRange(row_value)
                    if not range_value:
                        ERROR("pass range failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(range_value)
            # 列表
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntList:
                TRACE("----------------数字列表-----------------------")
                if row_value != "":
                    TRACE("----------------数字列表1-----------------------")
                    int_list = ParseIntList(row_value)
                    TRACE("----------------数字列表2-----------------------")
                    if not int_list:
                        ERROR("pass IntList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_list)
            #一对数字
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntPair:
                TRACE("----------------一对数字-----------------------")
                if row_value != "":
                    TRACE("----------------一对数字1-----------------------")
                    int_pair = ParseIntPair(row_value)
                    TRACE("----------------一对数字2-----------------------")
                    if not int_pair:
                        ERROR("pass IntPair failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_pair)
            # 数字对列表
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntPairList:
                if row_value != "":
                    int_pair_list = ParseIntPairList(row_value)
                    if not int_pair_list:
                        ERROR("pass IntPairList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_pair_list)
            # 数字对列表
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntTupleList:
                if row_value != "":
                    int_tuple_list = ParseIntTupleList(row_value)
                    if not int_tuple_list:
                        ERROR("pass IntTupleList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_tuple_list)
            # 数字列表列表
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntListList:
                if row_value != "":
                    int_list_list = ParseIntListList(row_value)
                    if not int_list_list:
                        ERROR("pass IntListList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_list_list)
            # 列表使用#隔开
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntListJingHao:
                TRACE("----------------数字列表-----------------------")
                if row_value != "":
                    TRACE("----------------数字列表1-----------------------")
                    int_list = ParseIntListJingHao(row_value)
                    TRACE("----------------数字列表2-----------------------")
                    if not int_list:
                        ERROR("pass IntList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_list)
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntListXiaHuaXian:
                TRACE("----------------数字列表-----------------------")
                if row_value != "":
                    TRACE("----------------数字列表1-----------------------")
                    int_list = ParseIntListXiaHuaXian(row_value)
                    TRACE("----------------数字列表2-----------------------")
                    if not int_list:
                        ERROR("pass IntList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_list)
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntListJingHaoMeiYuan:
                TRACE("----------------数字列表-----------------------")
                if row_value != "":
                    TRACE("----------------数字列表1-----------------------")
                    int_list = ParseIntListJingHaoMeiYuan(row_value)
                    TRACE("----------------数字列表2-----------------------")
                    if not int_list:
                        ERROR("pass IntList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_list)
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntListXiaHuaXianFenHao:
                TRACE("----------------数字列表-----------------------")
                if row_value != "":
                    TRACE("----------------数字列表1-----------------------")
                    int_list = ParseIntListXiaHuaXianFenHao(row_value)
                    TRACE("----------------数字列表2-----------------------")
                    if not int_list:
                        ERROR("pass IntList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_list)
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.StringList:
                TRACE("----------------数字列表-----------------------")
                if row_value != "":
                    TRACE("----------------数字列表1-----------------------")
                    int_list = ParseStringList(row_value)
                    TRACE("----------------数字列表2-----------------------")
                    if not int_list:
                        ERROR("pass IntList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_list)
            elif field_desc[ENUM_FIELD_TYPE] == table_common_pb2.IntSpecXiaHuaJinHao:
                TRACE("----------------数字列表-----------------------")
                if row_value != "":
                    TRACE("----------------数字列表1-----------------------")
                    int_list = ParseIntSpecXiaHuaJinHao(row_value)
                    TRACE("----------------数字列表2-----------------------")
                    if not int_list:
                        ERROR("pass IntList failed: " + row_value);
                        exit(1)
                    getattr(row, field_desc[ENUM_FIELD_NAME]).CopyFrom(int_list)


            else:
                ERROR("invalid field type: " + str(field_desc[ENUM_FIELD_TYPE]))
                exit(1)
        TRACE("------------------2---------------------")
        TRACE(str(row))
        TRACE("------------------3---------------------")

    f = open(tbl_file, "wb")
    f.write(row_array.SerializeToString())
    f.close()

if __name__ == "__main__":
    print "Hello world!"
    workbook_name = None
    sheet_name = None
    entry_name = None

    for arg in sys.argv[1:]:
        if arg[0] == "-":
            if arg == "-c":
                DUMP_CLIENT = True
            elif arg == "-v":
                ENABLE_VERBOSE = True
        else:
            if workbook_name == None:
                workbook_name = arg
            elif sheet_name == None:
                sheet_name = arg
            elif entry_name == None:
                entry_name = arg
            else:
                Usage()
                exit(1)
    if sheet_name == "":
        sheet_name = None

    if workbook_name == None:
        Usage()
        exit(2)

    INFO("Dumping Table [" + workbook_name + "]");
    if DUMP_CLIENT:
        INFO("!!! Running Dumping [CLIENT] Table !!!")

    msg="1235484554"

    print "Hello world!"
    try:
        Process(workbook_name, sheet_name, entry_name)
    except:
        ENABLE_VERBOSE = True
        Process(workbook_name,sheet_name,entry_name)
        INFO("FDJKLFJDS")
        exit(1)


    exit(0)


