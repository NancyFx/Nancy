require 'albacore'

# Forked from Albacore to add AssemblyInformationalVersion support
class CustomAssemblyInfo
  include Albacore::Task
  include Configuration::AssemblyInfo
  
  attr_accessor :input_file, :output_file, :language,
   :version, :title, :description, :custom_attributes,
   :copyright, :com_visible, :com_guid, :company_name, :product_name,
   :file_version, :trademark, :lang_engine, :assembly_informational_version
  
  attr_array :namespaces, :custom_data
  attr_hash :custom_attributes
  
  def initialize
    @namespaces = []
    @custom_data = []
    super()
    update_attributes assemblyinfo.to_hash
  end

  def use(file)
    @input_file = @output_file = file
  end
  
  def execute
    unless check_lang_engine then
      @lang_engine = from_language
    end
    write_assemblyinfo @output_file, @input_file
  end
  
  def write_assemblyinfo(assemblyinfo_file, input_file)
    valid = check_output_file assemblyinfo_file
    return if !valid

    input_data = read_input_file input_file
    asm_data = build_assembly_info_data input_data

    @logger.info "Generating Assembly Info File At: " + File.expand_path(assemblyinfo_file)
    File.open(assemblyinfo_file, 'w') do |f|      
      asm_data.each do |line|
        f.puts line
      end
    end
  end

  def read_input_file(file)
    data = []
    return data if file.nil?

    File.open(file, 'r') do |f|
        f.each_line do |line|
            data << line.strip
        end
    end

    data
  end
  
  def check_output_file(file)
    return true if file
    fail_with_message 'output_file cannot be nil'
    false
  end

  def from_language
    ({
      "F#" => lambda { FSharpEngine.new },
      "C#" => lambda { CSharpEngine.new },
      "C++.Net" => lambda { CppCliEngine.new },
      "VB.Net" => lambda { VbNetEngine.new }
    }[@language] || lambda { CSharpEngine.new }).call
  end

  def check_lang_engine
    !@lang_engine.nil?
  end

  def build_assembly_info_data(data)
    # data < []
    # requires: @lang_engine.nil? == false

    if data.empty?
        data = build_header
    end

    data = build_using_statements(data) + data

    build_attribute(data, "AssemblyTitle", @title)
    build_attribute(data, "AssemblyDescription", @description)
    build_attribute(data, "AssemblyCompany", @company_name)
    build_attribute(data, "AssemblyProduct", @product_name)
    
    build_attribute(data, "AssemblyCopyright", @copyright)
    build_attribute(data, "AssemblyTrademark", @trademark)
    
    build_attribute(data, "ComVisible", @com_visible)
    build_attribute(data, "Guid", @com_guid)
    
    build_attribute(data, "AssemblyVersion", @version)
    build_attribute(data, "AssemblyFileVersion", @file_version)
    build_attribute(data, "AssemblyInformationalVersion", @assembly_informational_version)
    
    data << ""
    if @custom_attributes != nil
      build_custom_attributes(data)
      data << ""
    end

    @custom_data.each do |cdata| 
      data << cdata unless data.include? cdata
    end
    
    data.concat build_footer

    chomp data
  end

  def chomp(ary)
    non_empty_rindex = ary.rindex {|line| !line.empty? } || 0
    ary.slice(0..non_empty_rindex)
  end

  def build_header
    @lang_engine.respond_to?(:before) ? [@lang_engine.before()] : []
  end

  def build_footer
    @lang_engine.respond_to?(:after) ? [@lang_engine.after()] : []
  end

  def build_attribute(data, attr_name, attr_data, allow_empty_args = false)
    if !allow_empty_args and attr_data.nil? then return end
    attr_value = @lang_engine.build_attribute(attr_name, attr_data)
    attr_re = @lang_engine.build_attribute_re(attr_name)
    result = nil
    @logger.debug "Build Assembly Info Attribute: " + attr_value
    data.each do |line|
        break unless result.nil?
        result = line.sub! attr_re, attr_value
    end
    data << attr_value if result.nil?
  end
  
  def build_using_statements(data)
    @namespaces = [] if @namespaces.nil?
    
    @namespaces << "System.Reflection"
    @namespaces << "System.Runtime.InteropServices"
    @namespaces.uniq!
    
    ns = []
    @namespaces.each do |n|
      ns << @lang_engine.build_using_statement(n) unless data.index { |l| l.match n }
    end
    
    ns
  end  

  def build_custom_attributes(data)
    @custom_attributes.each do |key, value|
      build_attribute(data, key, value, true)
    end
  end
  
end
